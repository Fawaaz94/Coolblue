namespace Application.DTOs;

public class InsuranceDto
{
    public int ProductId { get; set; }
    public decimal InsuranceValue { get; set; }
    public bool ProductTypeHasInsurance { get; set; }
    public string? ProductTypeName { get; set; }
    public decimal SalesPrice { get; set; }
}