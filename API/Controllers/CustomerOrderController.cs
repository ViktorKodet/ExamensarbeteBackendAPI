using API.Contexts;
using API.Models;
using API.Models.DTOs;
using Mapster;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerOrderController : ControllerBase
{
    private readonly HakimDbContext dbContext;

    public CustomerOrderController(HakimDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpPost]
    public IActionResult AddOrder([FromBody] CustomerOrderCreationDTO input)
    {
        City city = dbContext.Cities.FirstOrDefault(c => c.Name == input.Customer.City);
        if (city is null)
        {
            city = new City() { Name = input.Customer.City };
            dbContext.Cities.Add(city);
        }

        var customer = input.Customer.Adapt<Customer>();
        customer.City = city;
        var co = new CustomerOrder() { Customer = customer, ProductQuantities = input.Products };
        dbContext.Add(customer);
        dbContext.Add(co);
        dbContext.SaveChanges();
        return Ok("Order added.");
    }
}