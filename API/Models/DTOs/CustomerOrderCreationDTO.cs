using Newtonsoft.Json;

namespace API.Models.DTOs
{
    public class CustomerOrderCreationDTO
    {
        public CustomerCreationDTO Customer { get; set; }
        public ICollection<ProductQuantityDTO> Products { get; set; }
    }
}
