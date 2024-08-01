using Application.Surcharges.Queries;
using Domain.Entities;
using FluentAssertions;

namespace Application.Functional.Tests.Surcharges.Queries;

public class GetSurchargeRatesTests : BaseTestFixture
{ 
    private GetSurchargeRatesQueryHandler _handler;
 
   [SetUp]
   public void CreateHandler()
   { 
       _handler = new GetSurchargeRatesQueryHandler(_context);
   }
   
   [Test]
   public async Task Handle_ShouldReturnAllSurchargeRates()
   {
       // Arrange
       var surchargeRate1 = new SurchargeRate
       {
           Id = 1,
           ProductTypeId = 1,
           ProductTypeName = "Product 1",
           Rate = 5.0m,
           CreatedDate = DateTime.Now
       };
       var surchargeRate2 = new SurchargeRate
       {
           Id = 2,
           ProductTypeId = 2,
           ProductTypeName = "Product 2",
           Rate = 10.0m,
           CreatedDate = DateTime.Now
       };

       _context.SurchargeRates.AddRange(surchargeRate1, surchargeRate2);
       await _context.SaveChangesAsync(CancellationToken.None);

       var query = new GetSurchargeRatesQuery();

        // Act       
       var result = await _handler.Handle(query, CancellationToken.None);

       // Assert
       result.Should().NotBeNull();
       result.SurchargeRates.Should().HaveCount(2);
   }
   
   [Test]
   public async Task ShouldReturnEmptyListWhenNoSurchargeRates()
   {
       // Arrange
       var query = new GetSurchargeRatesQuery();

       // Act
       var result = await _handler.Handle(query, CancellationToken.None);

       // Assert
       result.Should().NotBeNull();
       result.SurchargeRates.Should().HaveCount(0);
   }
}