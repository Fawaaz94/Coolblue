namespace Domain.Entities;

public class ProductType
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool CanBeInsured { get; set; }
}