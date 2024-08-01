using Domain.Entities;

namespace Application.Interfaces;

public interface IInsuranceCalculator
{
    Task<decimal> CalculateInsuranceAsync(Product product, ProductType productType);
}