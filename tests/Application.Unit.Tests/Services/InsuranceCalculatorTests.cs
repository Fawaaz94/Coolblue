using Application.Interfaces;
using Application.Services;
using Domain.Common;
using Domain.Entities;
using FluentAssertions;

namespace Application.Unit.Tests.Services;

public class InsuranceCalculatorTests
{
    private IInsuranceCalculator _calculator;

    [SetUp]
    public void Setup()
    {
        _calculator = new InsuranceCalculator();
    }
    
    [Test]
    public async Task CalculateInsuranceAsync_SalesPriceLessThan500_ShouldReturn0()
    {
        // Arrange
        var product = new Product { SalesPrice = 499 };
        var productType = new ProductType { Name = CommonConstants.DigitalCameras };

        // Act
        var result = await _calculator.CalculateInsuranceAsync(product, productType);

        // Assert
        result.Should().Be(0);
    }
    
    [Test]
    public async Task CalculateInsuranceAsync_SalesPriceBetween500And1999_ShouldReturn1000()
    {
        // Arrange
        var product = new Product { SalesPrice = 1000 };
        var productType = new ProductType { Name = CommonConstants.DigitalCameras };
        
        // Act
        var result = await _calculator.CalculateInsuranceAsync(product, productType);

        // Assert
        result.Should().Be(1000);
    }
    
    [Test]
    public async Task CalculateInsuranceAsync_SalesPrice2000_ShouldReturn2000()
    {
        // Arrange
        var product = new Product { SalesPrice = 2000 };
        var productType = new ProductType { Name = "Desktop" };

        // Act
        var result = await _calculator.CalculateInsuranceAsync(product, productType);

        // Assert
        result.Should().Be(2000);
    }
    
    [Test]
    public async Task CalculateInsuranceAsync_SalesPriceMoreThan2000_ShouldReturn2000()
    {
        // Arrange
        var product = new Product { SalesPrice = 2001 };
        var productType = new ProductType { Name = "Desktop" };

        // Act
        var result = await _calculator.CalculateInsuranceAsync(product, productType);

        // Assert
        result.Should().Be(2000);
    }
    
    [Test]
    public async Task CalculateInsuranceAsync_LaptopUnder500_ShouldReturn500()
    {
        // Arrange
        var product = new Product { SalesPrice = 499 };
        var productType = new ProductType { Name = CommonConstants.Laptops };
        
        // Act
        var result = await _calculator.CalculateInsuranceAsync(product, productType);

        // Assert
        result.Should().Be(500);
    }
    
    [Test]
    public async Task CalculateInsuranceAsync_LaptopBetween500And2000_ShouldReturn1500()
    {
        // Arrange
        var product = new Product { SalesPrice = 1000 };
        var productType = new ProductType { Name = CommonConstants.Laptops };

        // Act
        var result = await _calculator.CalculateInsuranceAsync(product, productType);

        // Assert
        result.Should().Be(1500);
    }
    
    [Test]
    public async Task CalculateInsuranceAsync_LaptopAt1999_ShouldReturn1500()
    {
        // Arrange
        var product = new Product { SalesPrice = 1999 };
        var productType = new ProductType { Name = CommonConstants.Laptops };

        // Act
        var result = await _calculator.CalculateInsuranceAsync(product, productType);

        // Assert
        result.Should().Be(1500);
    }

    [Test]
    public async Task CalculateInsuranceAsync_LaptopAt2000_ShouldReturn2500()
    {
        // Arrange
        var product = new Product { SalesPrice = 2000 };
        var productType = new ProductType { Name = CommonConstants.Laptops };

        // Act
        var result = await _calculator.CalculateInsuranceAsync(product, productType);

        // Assert
        result.Should().Be(2500);
    }
    
    [Test]
    public async Task CalculateInsuranceAsync_LaptopMoreThan2000_ShouldReturn2500()
    {
        // Arrange
        var product = new Product { SalesPrice = 20001 };
        var productType = new ProductType { Name = CommonConstants.Laptops };

        // Act
        var result = await _calculator.CalculateInsuranceAsync(product, productType);

        // Assert
        result.Should().Be(2500);
    }
    
    [Test]
    public async Task CalculateInsuranceAsync_SmartphoneBetween500And1999_ShouldReturn1500()
    {
        // Arrange
        var product = new Product { SalesPrice = 1000 };
        var productType = new ProductType { Name = CommonConstants.Smartphones };

        // Act
        var result = await _calculator.CalculateInsuranceAsync(product, productType);

        // Assert
        result.Should().Be(1500);
    }
    
    [Test]
    public async Task CalculateInsuranceAsync_SmartphoneOver2000_ShouldReturn2500()
    {
        // Arrange
        var product = new Product { SalesPrice = 2001 };
        var productType = new ProductType { Name = CommonConstants.Smartphones };

        // Act
        var result = await _calculator.CalculateInsuranceAsync(product, productType);

        // Assert
        result.Should().Be(2500);
    }
    
    [Test]
    public async Task CalculateInsuranceAsync_ProductWithZeroPrice_ShouldReturn0()
    {
        // Arrange
        var product = new Product { SalesPrice = 0 };
        var productType = new ProductType { Name = CommonConstants.Laptops };
    
        // Act
        var result = await _calculator.CalculateInsuranceAsync(product, productType);
    
        // Assert
        result.Should().Be(0);
    }

    [Test]
    public async Task CalculateInsuranceAsync_ProductWithNegativePrice_ShouldReturn0()
    {
        var product = new Product { SalesPrice = -100 };
        var productType = new ProductType { Name = CommonConstants.Laptops };

        await FluentActions.Invoking((Func<Task>)Action)
            .Should().ThrowAsync<ArgumentException>().WithMessage("Sales price not valid");
        return;

        async Task Action() => await _calculator.CalculateInsuranceAsync(product, productType);
    }
}