namespace API.Models;

public class CustomerOrder
{
    public long Id { get; set; }
    public ICollection<ProductQuantity> ProductQuantities { get; set; }
    public DateOnly Date { get; set; }
    public Customer Customer { get; set; }
    public bool Sent { get; set; }
    public bool PaymentStatus { get; set; }
}