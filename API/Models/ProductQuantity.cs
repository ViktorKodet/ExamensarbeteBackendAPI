namespace API.Models;

public class ProductQuantity
{
    public long Id { get; set;}
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public ICollection<CustomerOrder> CustomerOrders { get; set; }

}