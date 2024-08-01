using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Surcharges.Queries;

public record GetSurchargeRatesQuery : IRequest<SurchargeRateVm>;

public class GetSurchargeRatesQueryHandler(IApplicationDbContext context) : IRequestHandler<GetSurchargeRatesQuery, SurchargeRateVm>
{
    public async Task<SurchargeRateVm> Handle(GetSurchargeRatesQuery request, CancellationToken cancellationToken)
    {
        return new SurchargeRateVm
        {
            SurchargeRates = await context.SurchargeRates
                .AsNoTracking()
                .ToListAsync(cancellationToken) 
        };
    }
}