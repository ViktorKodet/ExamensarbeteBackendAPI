namespace API.Models;

public class OutOfStock
{
    public long Id { get; set; }
    public Product Product { get; set; }
    public DateOnly Date { get; set; }

}