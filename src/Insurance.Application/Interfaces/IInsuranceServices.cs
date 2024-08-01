using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IInsuranceServices
{
    Task<InsuranceDto> CalculateProductInsuranceAsync(int productId);

    Task<OrderInsuranceDto> CalculateOrderInsuranceAsync(IEnumerable<Product> productIds);
}