namespace API.Models.DTOs
{
    public class ProductUpdateDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public string? Picture { get; set; }
        public long? CategoryId { get; set; }
        public long? CompanyId { get; set; }
    }
}
