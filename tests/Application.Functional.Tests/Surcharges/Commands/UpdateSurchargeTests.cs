using Application.Surcharges.Commands;
using Ardalis.GuardClauses;
using Domain.Entities;
using FluentAssertions;

namespace Application.Functional.Tests.Surcharges.Commands;

public class UpdateSurchargeTests : BaseTestFixture
{
    private UpdateSurchargeCommandHandler _handler;
    
    [SetUp]
    public void CreateHandler()
    {
        _handler = new UpdateSurchargeCommandHandler(_context);
    }

    [Test]
    public async Task ShouldUpdateSurchargeRate()
    {
        var existingEntity = new SurchargeRate
        {
            Id = 1,
            ProductTypeId = 1,
            ProductTypeName = "Old Product",
            Rate = 4.0m,
            CreatedDate = DateTime.Now
        };
        
        _context.SurchargeRates.Add(existingEntity);
        await _context.SaveChangesAsync(CancellationToken.None);
        
        var command = new UpdateSurchargeCommand
        {
            Id = 1,
            ProductTypeId = 2,
            ProductTypeName = "Updated Product",
            Rate = 6.0m
        };
        
        await _handler.Handle(command, CancellationToken.None);
        
        var updatedEntity = await _context.SurchargeRates.FindAsync(command.Id);
        updatedEntity.Should().NotBeNull();
        updatedEntity?.ProductTypeId.Should().Be(command.ProductTypeId);
        updatedEntity?.ProductTypeName.Should().Be(command.ProductTypeName);
        updatedEntity?.Rate.Should().Be(command.Rate);
    }

    [Test]
    public async Task Handle_ShouldNotUpdateNonexistentSurchargeRate()
    {
        var command = new UpdateSurchargeCommand
        {
            Id = 99, // Nonexistent Id
            ProductTypeId = 2,
            ProductTypeName = "Nonexistent Product",
            Rate = 6.0m
        };

        await FluentActions.Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Queried object entity was not found, Key: {command.Id}");
    }
}