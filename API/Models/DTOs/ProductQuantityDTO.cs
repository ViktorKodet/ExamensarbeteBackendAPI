using Newtonsoft.Json;

namespace API.Models.DTOs
{
    public class ProductQuantityDTO
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
