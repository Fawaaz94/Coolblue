using System.Net;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace Application.Behavioural.Tests.Services;

public class InsuranceServicesTests
{
    private Mock<IInsuranceCalculator> _insuranceCalculatorMock;
    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private Mock<ILogger<InsuranceServices>> _loggerMock;
    private Mock<IApplicationDbContext> _contextMock;
    private InsuranceServices _insuranceServices;
    
    [SetUp]
    public void SetUp()
    {
        _insuranceCalculatorMock = new Mock<IInsuranceCalculator>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<InsuranceServices>>();
        _contextMock = new Mock<IApplicationDbContext>();

        _insuranceServices = new InsuranceServices(
            _insuranceCalculatorMock.Object,
            _httpClientFactoryMock.Object,
            _loggerMock.Object,
            _contextMock.Object
        );
    }
    
    [Test]
        public async Task CalculateProductInsuranceAsync_ShouldReturnInsuranceDto_WhenProductCanBeInsured()
        {
            // Arrange
            var productId = 1;
            var product = new Product { Id = productId, ProductTypeId = 1, SalesPrice = 100 };
            var productType = new ProductType { Id = 1, Name = "Electronics", CanBeInsured = true };
            var insuranceValue = 50m;
            var surchargeRate = 10m;

            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(new Mock<HttpClient>().Object);

            _insuranceCalculatorMock.Setup(x => x.CalculateInsuranceAsync(It.IsAny<Product>(), It.IsAny<ProductType>()))
                .ReturnsAsync(insuranceValue);

            _contextMock.Setup(x => x.SurchargeRates)
                .Returns(new List<SurchargeRate> { new SurchargeRate { ProductTypeId = 1, Rate = surchargeRate } }.AsQueryable());

            // Mock GetProductAsync and ListProductTypesAsync methods
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new Mock<HttpClient>().Object);

            var jsonProduct = JsonConvert.SerializeObject(product);
            var jsonProductTypes = JsonConvert.SerializeObject(new List<ProductType> { productType });

            var mockHttpClient = new Mock<HttpClient>();
            mockHttpClient.Setup(c => c.GetAsync("/products/1")).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonProduct)
            });
            mockHttpClient.Setup(c => c.GetAsync("/product_types")).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonProductTypes)
            });

            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient.Object);

            // Act
            var result = await _insuranceServices.CalculateProductInsuranceAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(productId, result.ProductId);
            Assert.AreEqual(insuranceValue + surchargeRate, result.InsuranceValue);
            Assert.IsTrue(result.ProductTypeHasInsurance);
        }
}