using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

public class CustomerOrder
{
    public long Id { get; set; }
    public virtual ICollection<ProductQuantity> ProductQuantities { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public Customer Customer { get; set; }
    public bool Sent { get; set; }
    public bool PaymentStatus { get; set; }
}