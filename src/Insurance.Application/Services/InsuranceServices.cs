using Application.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Application.Services;

public class InsuranceServices(
        IInsuranceCalculator insuranceCalculator, 
        IHttpClientFactory httpClientFactory, 
        ILogger<InsuranceServices> logger,
        IApplicationDbContext context) : IInsuranceServices
{
    public async Task<InsuranceDto> CalculateProductInsuranceAsync(int productId)
    {
        try
        {
            const decimal noInsuranceValue = 0;
            
            var productTypes = await ListProductTypesAsync();
            var product = await GetProductAsync(productId);
            
            if (product == null || productTypes == null)
                return new InsuranceDto();
            
            var productType = productTypes.FirstOrDefault(pt => pt.Id == product.ProductTypeId && pt.CanBeInsured);

            if (productType is not { CanBeInsured: true })
                return new InsuranceDto
                {
                    ProductId = productId,
                    InsuranceValue = noInsuranceValue,
                    ProductTypeHasInsurance = false
                };
            
            var insuranceValue = await insuranceCalculator.CalculateInsuranceAsync(product, productType);

            if (insuranceValue >= 500)
            {
                var surchargeRate = await AddSurcharge(productType.Id);
                insuranceValue += surchargeRate;
            }

            var productTypeHasInsurance = insuranceValue > 0;
            
            return new InsuranceDto
            {
                ProductId = productId,
                InsuranceValue = insuranceValue,
                ProductTypeHasInsurance = productTypeHasInsurance,
                SalesPrice = product.SalesPrice,
                ProductTypeName = productType.Name
            };
        }
        catch (HttpRequestException httpEx)
        {
            logger.LogError(httpEx, "HTTP Request error occurred while calculating insurance for ProductId: {ProductId}", productId);
            throw;
        }
        catch (JsonSerializationException jsonEx)
        {
            logger.LogError(jsonEx, "JSON Serialization error occurred while calculating insurance for ProductId: {ProductId}", productId);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred while calculating insurance for ProductId: {ProductId}", productId);
            throw;
        }
    }

    public async Task<OrderInsuranceDto> CalculateOrderInsuranceAsync(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);

        const string digitalCameras = "Digital cameras";
        
        var productList = products.ToList();
        
        var orderInsurance = new OrderInsuranceDto
        {
            ProductInsurances = new List<InsuranceDto>(),
            TotalInsuranceValue = 0
        };
        
        var productTypes = await ListProductTypesAsync();
        
        var digitalCameraType = productTypes.FirstOrDefault(pt => pt.Name == digitalCameras);

        var tasks = productList.Select(async product =>
        {
            try
            {
                var productType = productTypes.FirstOrDefault(pt => pt.Id == product.ProductTypeId);
                var productInsurance = await CalculateProductInsuranceAsync(product.Id);

                var surchargeRate = await AddSurcharge(productType.Id);
                
                productInsurance.InsuranceValue += surchargeRate;
                
                if (productType.Id == digitalCameraType?.Id)
                {
                    productInsurance.InsuranceValue += 500;
                }

                return productInsurance;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error calculating insurance for ProductId: {ProductId}", product.Id);
                return new InsuranceDto();
            }
        });
        
        var results = await Task.WhenAll(tasks);
        
        foreach (var result in results)
        {
            orderInsurance.ProductInsurances.Add(result);
            orderInsurance.TotalInsuranceValue += result.InsuranceValue;
        }

        return orderInsurance;
    }
    
    private async Task<IEnumerable<ProductType>?> ListProductTypesAsync()
    {
        try
        {
            var client = httpClientFactory.CreateClient(CommonConstants.InsuranceApi);
            var response = await client.GetAsync("/product_types");
            response.EnsureSuccessStatusCode(); // Ensure the response status is 2xx
            
            var productTypesJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<ProductType>>(productTypesJson);
        }
        catch (HttpRequestException httpEx)
        {
            logger.LogError(httpEx, "Error fetching product types from API");
            throw;
        }
        catch (JsonSerializationException jsonEx)
        {
            logger.LogError(jsonEx, "Error deserializing product types response");
            throw;
        }
    }

    private async Task<Product?> GetProductAsync(int productId)
    {
        try
        {
            var client = httpClientFactory.CreateClient(CommonConstants.InsuranceApi);
            var response = await client.GetAsync($"/products/{productId:G}");
            response.EnsureSuccessStatusCode();
            
            var productJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Product>(productJson);
        }
        catch (HttpRequestException httpEx)
        {
            logger.LogError(httpEx, "Error fetching product with ProductId: {ProductId} from API", productId);
            throw;
        }
        catch (JsonSerializationException jsonEx)
        {
            logger.LogError(jsonEx, "Error deserializing product response for ProductId: {ProductId}", productId);
            throw;
        }
    }

    private Task<decimal> AddSurcharge(int productTypeId)
    {
        try
        {
            var surcharge = context.SurchargeRates
                .FirstOrDefault(s => s.ProductTypeId == productTypeId);
            return Task.FromResult(surcharge?.Rate ?? 0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching surcharge rate for ProductTypeId: {ProductTypeId}", productTypeId);
            throw;
        }
    }
}