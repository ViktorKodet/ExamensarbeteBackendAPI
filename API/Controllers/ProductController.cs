//using System.Data.Entity;
using API.Contexts;
using API.Models;
using Mapster;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableCors("CorsPolicy")]
public class ProductController : ControllerBase
{
    private readonly HakimDbContext dbContext;

    public ProductController(HakimDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpPost]
    public IActionResult AddProduct([FromBody] ProductCreationDTO p)
    {
        var company = dbContext.Companies.FirstOrDefault(c => c.Id == p.CompanyId);
        var category = dbContext.Categories.FirstOrDefault(c => c.Id == p.CategoryId);
        if (company is null)
        {
            return BadRequest($"No company with id {p.CompanyId} found. Please validate input and try again.");
        }
        if (category is null)
        {
            return BadRequest($"No category with id {p.CategoryId} found. Please validate input and try again.");
        }

        var product = p.Adapt<Product>();
        product.Company = company;
        product.Category = category;
        dbContext.Products.Add(product);
        dbContext.SaveChanges();
        return Ok("Product added.");
    }

    [HttpPut("toggle/id")]
    public IActionResult ToggleProductActive(long id)
    {
        var product = dbContext.Products.FirstOrDefault(x => x.Id == id);
        if (product is null)
        {
            return BadRequest($"No product with Id {id} found. Please validate your input and try again.");
        }

        product.Active = !product.Active;
        dbContext.SaveChanges();
        return Ok($"{product.Name} active-status successfully updated to {product.Active}.");
    }

    [HttpPut("restock/id/quantity")]
    public IActionResult RestockProductQuantity(long id, int quantity)
    {
        var product = dbContext.Products.FirstOrDefault(x => x.Id == id);
        if (product is null)
        {
            return BadRequest($"No product with Id {id} found. Please validate your input and try again.");
        }

        var previousQuantity = product.Quantity;
        product.Quantity += quantity;
        dbContext.SaveChanges();
        return Ok($"{product.Name} stock increased from {previousQuantity} to {product.Quantity}.");
    }

    [HttpGet("all/active")]
    public IActionResult GetAllActiveProducts()
    {
        return Ok(dbContext.Products
            .Where(p => p.Active)
            .Include(p => p.Category)    //TODO category och company kommer tillbaks som null
            .Include(p => p.Company)    //TODO löste genom att byta till från system.data.entity till efcore
            .ToList());

        /*
         * Funkar bra på swagger etc men funkar inte när jag försöker hämta från hemsidan.
         * läst lite om CORS och skit, kan också ha med typ authorisering eller något. ev att man inte får ha https eller något
         * får klura vidare, se om jag kan få bättre felkoder någonstans på hemsidan eller i vsc.
         * testa att öppna konsollen på hemsidan i browsern eller något, kommer inte ihåg hur man gör
         */
    }

    [HttpGet("all/inactive")]
    public IActionResult GetAllInactiveProducts()
    {
        return Ok(dbContext.Products
            .Where(p => !p.Active)
            .ToList());
    }
}