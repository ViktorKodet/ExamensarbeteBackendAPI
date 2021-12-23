namespace API.Models;

public class Customer
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public City City { get; set; }
    public string ZipCode { get; set; }
    public string Adress { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? Password { get; set; }
    public bool IsAdmin { get; set; } = false;

}