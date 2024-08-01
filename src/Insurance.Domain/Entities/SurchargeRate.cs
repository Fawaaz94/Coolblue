namespace Domain.Entities;

public class SurchargeRate
{
    public int Id { get; set; }
    public int ProductTypeId { get; set; }
    public string? ProductTypeName { get; set; }
    public decimal Rate { get; set; }
    public DateTime CreatedDate { get; set; }
}