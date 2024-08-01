using Application.Surcharges.Commands;
using FluentAssertions;

namespace Application.Functional.Tests.Surcharges.Commands;

public class CreateSurchargeTests : BaseTestFixture
{
    private CreateSurchargeCommandHandler _handler;
    
    [SetUp]
    public void CreateHandler()
    {
        _handler = new CreateSurchargeCommandHandler(_context);
    }

    [Test]
    public async Task ShouldCreateSurchargeRate()
    {
        var command = new CreateSurchargeCommand
        {
            ProductTypeId = 1,
            ProductTypeName = "Test Product",
            Rate = 5.0m
        };
        
        var surchargeId = await _handler.Handle(command, CancellationToken.None);
        
        var surcharge = await _context.SurchargeRates.FindAsync(surchargeId);

        surcharge.Should().NotBeNull();
        surcharge?.ProductTypeId.Should().Be(command.ProductTypeId);
        surcharge?.ProductTypeName.Should().Be(command.ProductTypeName);
        surcharge?.Rate.Should().Be(command.Rate);
    }
}