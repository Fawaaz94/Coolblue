namespace Application.DTOs;

public class OrderInsuranceDto
{
    public IList<InsuranceDto> ProductInsurances { get; set; } = new List<InsuranceDto>();
    public decimal TotalInsuranceValue { get; set; }
}