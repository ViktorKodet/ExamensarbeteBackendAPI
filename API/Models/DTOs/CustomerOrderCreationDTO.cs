namespace API.Models.DTOs
{
    public class CustomerOrderCreationDTO
    {
        public CustomerCreationDTO Customer { get; set; }
        public ICollection<ProductQuantity> Products { get; set; }
    }
}
