namespace API.Models;

public class Product
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string Picture { get; set; }
    public Category Category { get; set; }
    public Company Company { get; set; }
    public bool Active { get; set; }

}