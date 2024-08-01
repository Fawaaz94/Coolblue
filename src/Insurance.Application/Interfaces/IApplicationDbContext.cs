using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<SurchargeRate> SurchargeRates { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}