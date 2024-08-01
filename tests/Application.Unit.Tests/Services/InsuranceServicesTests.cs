using System.Net;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace Application.Unit.Tests.Services
{
    [TestFixture]
    public class InsuranceServicesTests
    {
        private Mock<IInsuranceCalculator> _mockInsuranceCalculator;
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private Mock<ILogger<InsuranceServices>> _mockLogger;
        private Mock<IApplicationDbContext> _mockContext;
        private InsuranceServices _insuranceServices;

        [SetUp]
        public void SetUp()
        {
            _mockInsuranceCalculator = new Mock<IInsuranceCalculator>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<InsuranceServices>>();
            _mockContext = new Mock<IApplicationDbContext>();

            _insuranceServices = new InsuranceServices(
                _mockInsuranceCalculator.Object,
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _mockContext.Object);

            SetupMockHttpClient();
            SetupMockContext();
        }

        [Test]
        public async Task CalculateProductInsuranceAsync_ProductTypeCannotBeInsured_ReturnsZeroInsurance()
        {
            // Arrange
            const int productId = 837856;
            _mockInsuranceCalculator.Setup(x => x.CalculateInsuranceAsync(It.IsAny<Product>(), It.IsAny<ProductType>()))
                .ReturnsAsync(0);

            // Act
            var result = await _insuranceServices.CalculateProductInsuranceAsync(productId);

            // Assert
            result.InsuranceValue.Should().Be(0);
            result.ProductTypeHasInsurance.Should().BeFalse();
        }

        [Test]
        public async Task CalculateProductInsuranceAsync_ProductTypeCanBeInsured_ReturnsCalculatedInsuranceWithSurcharge50()
        {
            // Arrange
            const int productId = 837856;
            _mockInsuranceCalculator.Setup(x => x.CalculateInsuranceAsync(It.IsAny<Product>(), It.IsAny<ProductType>()))
                .ReturnsAsync(500m);

            // Act
            var result = await _insuranceServices.CalculateProductInsuranceAsync(productId);

            // Assert
            result.InsuranceValue.Should().Be(550m);
            result.ProductTypeHasInsurance.Should().BeTrue();
        }

        [Test]
        public async Task CalculateOrderInsuranceAsync_CalculatesInsuranceForAllProductsPlusSurcharge()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { Id = 837856, ProductTypeId = 21, SalesPrice = 300 },
                new() { Id = 837857, ProductTypeId = 124, SalesPrice = 500 }
            };

            _mockInsuranceCalculator.Setup(x => x.CalculateInsuranceAsync(It.Is<Product>(p => p.ProductTypeId == 21), It.IsAny<ProductType>()))
                .ReturnsAsync(500);
            _mockInsuranceCalculator.Setup(x => x.CalculateInsuranceAsync(It.Is<Product>(p => p.ProductTypeId == 124), It.IsAny<ProductType>()))
                .ReturnsAsync(0);
            
            // Act
            var result = await _insuranceServices.CalculateOrderInsuranceAsync(products);

            // Assert
            result.ProductInsurances.Should().HaveCount(2);
            result.TotalInsuranceValue.Should().Be(600);
        }

        [Test]
        public async Task CalculateOrderInsuranceAsync_AppliesDigitalCameraSurcharge()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { Id = 837858, ProductTypeId = 35, SalesPrice = 1000 }
            };

            _mockInsuranceCalculator.Setup(x => x.CalculateInsuranceAsync(It.IsAny<Product>(), It.IsAny<ProductType>()))
                .ReturnsAsync(0);

            // Act
            var result = await _insuranceServices.CalculateOrderInsuranceAsync(products);

            // Assert
            result.ProductInsurances.Should().HaveCount(1);
            result.TotalInsuranceValue.Should().Be(0); // Digital camera surcharge
        }

        private void SetupMockContext()
        {
            var surchargeRates = new List<SurchargeRate>
            {
                new() { ProductTypeId = 21, Rate = 50 },
                new() { ProductTypeId = 124, Rate = 30 },
                new() { ProductTypeId = 35, Rate = 500 }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<SurchargeRate>>();
            mockDbSet.As<IQueryable<SurchargeRate>>().Setup(m => m.Provider).Returns(surchargeRates.Provider);
            mockDbSet.As<IQueryable<SurchargeRate>>().Setup(m => m.Expression).Returns(surchargeRates.Expression);
            mockDbSet.As<IQueryable<SurchargeRate>>().Setup(m => m.ElementType).Returns(surchargeRates.ElementType);
            mockDbSet.As<IQueryable<SurchargeRate>>().Setup(m => m.GetEnumerator()).Returns(surchargeRates.GetEnumerator());

            _mockContext.Setup(c => c.SurchargeRates).Returns(mockDbSet.Object);
        }

        private void SetupMockHttpClient()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().EndsWith("/product_types")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(new List<ProductType>
                    {
                        new() { Id = 21, Name = "Laptop", CanBeInsured = true },
                        new() { Id = 124, Name = "Washing machines", CanBeInsured = true },
                        new() { Id = 35, Name = "Digital cameras", CanBeInsured = true }
                    }))
                });

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("/products/837856")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(new Product
                    {
                        Id = 837856,
                        ProductTypeId = 21,
                        SalesPrice = 300
                    }))
                });

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost:5002")
            };

            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        }
    }
}
