using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Surcharges.Commands;

public record CreateSurchargeCommand : IRequest<int>
{
    public int ProductTypeId { get; init; }
    public string? ProductTypeName { get; init; }
    public decimal Rate { get; init; }
}

public class CreateSurchargeCommandHandler(IApplicationDbContext context): IRequestHandler<CreateSurchargeCommand, int>
{
    public async Task<int> Handle(CreateSurchargeCommand request, CancellationToken cancellationToken)
    {
        var entity = new SurchargeRate
        {
            ProductTypeId = request.ProductTypeId,
            ProductTypeName = request.ProductTypeName,
            Rate = request.Rate,
            CreatedDate = DateTime.Now
        };

        context.SurchargeRates.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

