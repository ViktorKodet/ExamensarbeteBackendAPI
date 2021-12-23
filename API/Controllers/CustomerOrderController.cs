using System.Data.Entity;
using API.Configs;
using API.Contexts;
using API.Models;
using API.Models.DTOs;
using Mapster;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableCors("CorsPolicy")]
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

        var productQuantities = new List<ProductQuantity>();
        foreach (var pqd in input.Products)
        {
            productQuantities.Add(new ProductQuantity(){Quantity = pqd.Quantity, Product = dbContext.Products.Where(p => p.Id == pqd.ProductId).FirstOrDefault()});
        }

        var customer = input.Customer.Adapt<Customer>(CustomerMapping.GetCustomerCreationDtoToCustomerMappingConfig());
        customer.City = city;
        var co = new CustomerOrder() { Customer = customer, ProductQuantities = productQuantities };  //TODO något sexigt sätt att få productquantities från bara productid och quantity
        dbContext.Add(customer);
        dbContext.Add(co);
        dbContext.SaveChanges();
        return Ok("Order added.");
    }
}