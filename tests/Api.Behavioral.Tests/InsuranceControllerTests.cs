using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Insurance.Api.Controllers;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Behavioral.Tests
{
    [TestFixture]
    public class InsuranceControllerTests
    {
        private Mock<IInsuranceServices> _insuranceServicesMock;
        private InsuranceController _controller;
        
        [SetUp]
        public void Setup()
        {
            _insuranceServicesMock = new Mock<IInsuranceServices>();
            _controller = new InsuranceController();
        }

        [Test]
        public async Task CalculateProductInsurance_ShouldReturnOkResult_WhenInsuranceIsCalculated()
        {
            // Arrange
            const int productId = 1;
            var insuranceDto = new InsuranceDto { ProductId = productId, InsuranceValue = 50, ProductTypeHasInsurance = true };

            _insuranceServicesMock.Setup(x => x.CalculateProductInsuranceAsync(productId))
                .ReturnsAsync(insuranceDto);

            // Act
            var result = await _controller.CalculateProductInsurance(_insuranceServicesMock.Object, productId);

            // Assert
            result.Should().BeOfType<Ok<InsuranceDto>>()
                .Which.Value.Should().BeEquivalentTo(insuranceDto);
        }

        [Test]
        public async Task CalculateOrderInsurance_ShouldReturnOkResult_WhenOrderInsuranceIsCalculated()
        {
            // Arrange
            var products = new List<Product> { new() { Id = 1, ProductTypeId = 1 } };
            var orderInsuranceDto = new OrderInsuranceDto
            {
                ProductInsurances = new List<InsuranceDto> { new() { ProductId = 1, InsuranceValue = 50, ProductTypeHasInsurance = true } },
                TotalInsuranceValue = 50
            };

            var request = new OrderInsuranceRequest(products);

            _insuranceServicesMock.Setup(x => x.CalculateOrderInsuranceAsync(products))
                .ReturnsAsync(orderInsuranceDto);

            // Act
            var result = await _controller.CalculateOrderInsurance(_insuranceServicesMock.Object, request);

            // Assert
            result.Should().BeOfType<Ok<OrderInsuranceDto>>()
                .Which.Value.Should().BeEquivalentTo(orderInsuranceDto);
        }

        [Test]
        public async Task CalculateOrderInsurance_ShouldReturnBadRequest_WhenOrderRequestIsInvalid()
        {
            // Arrange
            var invalidRequest = new OrderInsuranceRequest(null); // Null list of products

            // Act
            var result = await _controller.CalculateOrderInsurance(_insuranceServicesMock.Object, invalidRequest);

            // Assert
            result.Should().BeOfType<BadRequest>();
        }
    }
}
