using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

public class OutOfStock
{
    public long Id { get; set; }
    public Product Product { get; set; }
    public DateTime Date { get; set; }

}