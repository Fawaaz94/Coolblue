using Application.Interfaces;
using Ardalis.GuardClauses;
using MediatR;

namespace Application.Surcharges.Commands;

public class UpdateSurchargeCommand : IRequest
{
    public int Id { get; init; }
    public int ProductTypeId { get; init; }
    public string? ProductTypeName { get; init; }
    public decimal Rate { get; init; }
}

public class UpdateSurchargeCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateSurchargeCommand>
{
    public async Task Handle(UpdateSurchargeCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.SurchargeRates
            .FindAsync(new object[] { request.Id }, cancellationToken);
        
        Guard.Against.NotFound(request.Id, entity);

        entity.ProductTypeId = request.ProductTypeId;
        entity.ProductTypeName = request.ProductTypeName;
        entity.Rate = request.Rate;
        
        await context.SaveChangesAsync(cancellationToken);
    }
}