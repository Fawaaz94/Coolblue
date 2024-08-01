using Application.Interfaces;
using Domain.Common;
using Domain.Entities;

namespace Application.Services;

public class InsuranceCalculator : IInsuranceCalculator
{
    public async Task<decimal> CalculateInsuranceAsync(Product product, ProductType productType)
    {
        if (product.SalesPrice < 0)
            throw new ArgumentException("Sales price not valid");
        
        var insuranceValue = product.SalesPrice switch
        {
            0 => 0,
            < 500
                => productType.Name is CommonConstants.Laptops ? 500 : 0,
            >= 500 and < 2000
                => AddExtraInsuranceCost(productType) ? 1500 : 1000,
            >= 2000
                => AddExtraInsuranceCost(productType) ? 2500 : 2000
        };

        return await Task.FromResult<decimal>(insuranceValue);
    }

    private static bool AddExtraInsuranceCost(ProductType productType)
    {
        return productType.Name is CommonConstants.Laptops or CommonConstants.Smartphones;
    }
}