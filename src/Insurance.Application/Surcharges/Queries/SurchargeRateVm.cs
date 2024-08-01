using Domain.Entities;

namespace Application.Surcharges.Queries;

public class SurchargeRateVm
{
    public IReadOnlyCollection<SurchargeRate> SurchargeRates { get; init; }
}