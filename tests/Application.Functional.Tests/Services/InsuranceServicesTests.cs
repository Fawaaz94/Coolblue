using Application.Functional.Tests.Mocks;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Application.Functional.Tests.Services;

[TestFixture]
public class InsuranceServicesTests : BaseTestFixture
{
    private IServiceProvider _serviceProvider;
    private IInsuranceServices _insuranceServices;
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    
    [SetUp]
    public async Task SetUp()
    {
        await TestSetUp();
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ProductApiAddress", "http://localhost:5002" }
            })
            .Build();
        
        var services = new ServiceCollection();
        services.AddApplicationServices(configuration);
        
        services.AddSingleton<IApplicationDbContext>(_context);
        
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpClientFactory.SetupMockHttpClient("http://localhost:5002");
        services.AddSingleton(_mockHttpClientFactory.Object);

        _serviceProvider = services.BuildServiceProvider();
        _insuranceServices = _serviceProvider.GetRequiredService<IInsuranceServices>();

        await SeedTestDataAsync();
    }
    
    [Test]
    public async Task CalculateProductInsuranceAsync_ValidProduct_ReturnsCorrectInsurance()
    {
        const int productId = 837856;
        
        var result = await _insuranceServices.CalculateProductInsuranceAsync(productId);
        
        result.Should().NotBeNull();
        result.ProductId.Should().Be(productId);
        result.InsuranceValue.Should().BeGreaterThan(0);
        result.ProductTypeHasInsurance.Should().BeTrue();
    }

    [Test]
    public async Task CalculateOrderInsuranceAsync_ValidProducts_ReturnsCorrectTotalInsurance()
    {
        var products = new List<Product>
        {
            new () { Id = 837856, ProductTypeId = 21, SalesPrice = 300 },
            new () { Id = 572770, ProductTypeId = 124, SalesPrice = 475 },
            new () { Id = 735296, ProductTypeId = 35, SalesPrice = 300 }
        };
        
        var result = await _insuranceServices.CalculateOrderInsuranceAsync(products);
        
        result.Should().NotBeNull();
        result.ProductInsurances.Should().HaveCount(products.Count);
        result.TotalInsuranceValue.Should().BeGreaterThan(0);
    }
    
    private async Task SeedTestDataAsync()
    {
        _context.SurchargeRates.Add(
            new SurchargeRate
            {
                ProductTypeId = 21, 
                Rate = 300,
                ProductTypeName = CommonConstants.Laptops,
                CreatedDate = DateTime.Now
            });
        
        await _context.SaveChangesAsync();
    }
    
    [TearDown]
    public new async Task TearDown()
    {
        await base.TearDown();

        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        _context = null;
    }
}