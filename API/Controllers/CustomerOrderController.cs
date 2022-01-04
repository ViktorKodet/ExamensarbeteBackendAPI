using System.Data.Entity;
using API.Configs;
using API.Contexts;
using API.Models;
using API.Models.DTOs;
using Mapster;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Wrap;
using API.ResiliencePolicies;
using Timeout = API.ResiliencePolicies.Timeout;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableCors("CorsPolicy")]
public class CustomerOrderController : ControllerBase
{
    private readonly HakimDbContext dbContext;
    private readonly PolicyWrap _policyWrap;

    public CustomerOrderController(HakimDbContext dbContext)
    {
        this.dbContext = dbContext;
        var retry = Retry.GetPolicy();
        var timeout = Timeout.GetPolicy();
        _policyWrap = Policy.Wrap(retry, timeout);
    }

    [HttpPost]
    public IActionResult AddOrder([FromBody] CustomerOrderCreationDTO input)
    {
        City city = _policyWrap.Execute(() => dbContext.Cities.FirstOrDefault(c => c.Name == input.Customer.City));
        if (city is null)
        {
            city = new City() { Name = input.Customer.City };
            _policyWrap.Execute(() => dbContext.Cities.Add(city));
        }

        var productQuantities = new List<ProductQuantity>();
        foreach (var pqd in input.Products)
        {
            _policyWrap.Execute(() => productQuantities.Add(new ProductQuantity(){Quantity = pqd.Quantity, Product = dbContext.Products.Where(p => p.Id == pqd.ProductId).FirstOrDefault()}));
        }

        var customer = input.Customer.Adapt<Customer>(CustomerMapping.GetCustomerCreationDtoToCustomerMappingConfig());
        customer.City = city;
        var co = new CustomerOrder() { Customer = customer, ProductQuantities = productQuantities };  //TODO något sexigt sätt att få productquantities från bara productid och quantity
        _policyWrap.Execute(() => dbContext.Add(customer));
        _policyWrap.Execute(() => dbContext.Add(co));
        _policyWrap.Execute(() => dbContext.SaveChanges());
        return Ok("Order added.");
    }
}