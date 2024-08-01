namespace Application.Interfaces;

public interface IAdminServices
{
    Task CreateSurchargeRatesAsync(int id, int productTypeId, string productTypeName, decimal rate, CancellationToken cancellationToken);
}