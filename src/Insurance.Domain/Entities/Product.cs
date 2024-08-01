namespace Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public int ProductTypeId { get; set; }
    public string? Name { get; set; }
    public int SalesPrice { get; set; }
}