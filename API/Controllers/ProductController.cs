//using System.Data.Entity;
using API.Contexts;
using API.Models;
using API.ResiliencePolicies;
using Mapster;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using Polly.Wrap;
using Timeout = API.ResiliencePolicies.Timeout;
using API.Configs;
using API.Models.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableCors("CorsPolicy")]
public class ProductController : ControllerBase
{
    private readonly HakimDbContext dbContext;
    private readonly PolicyWrap _policyWrap;
    public ProductController(HakimDbContext dbContext)
    {
        this.dbContext = dbContext;
        var retry = Retry.GetPolicy();
        var timeout = Timeout.GetPolicy();
        _policyWrap = Policy.Wrap(retry, timeout);
    }

    [HttpPost]
    public IActionResult AddProduct([FromBody] ProductCreationDTO p)
    {
        var company = _policyWrap.Execute(() => dbContext.Companies.FirstOrDefault(c => c.Id == p.CompanyId));
        if (company is null)
        {
            return BadRequest($"No company with id {p.CompanyId} found. Please validate input and try again.");
        }

        var category = _policyWrap.Execute(() => dbContext.Categories.FirstOrDefault(c => c.Id == p.CategoryId));
        if (category is null)
        {
            return BadRequest($"No category with id {p.CategoryId} found. Please validate input and try again.");
        }

        var product = p.Adapt<Product>();
        product.Company = company;
        product.Category = category;
        _policyWrap.Execute(() => dbContext.Products.Add(product));
        _policyWrap.Execute(() => dbContext.SaveChanges());
        return Ok("Product added.");
    }

    [HttpPut("toggle/id")]
    public IActionResult ToggleProductActive(long id)
    {
        var product = _policyWrap.Execute(() => dbContext.Products.FirstOrDefault(x => x.Id == id));
        if (product is null)
        {
            return BadRequest($"No product with Id {id} found. Please validate your input and try again.");
        }

        product.Active = !product.Active;
        _policyWrap.Execute(() => dbContext.SaveChanges());
        return Ok($"{product.Name} active-status successfully updated to {product.Active}.");
    }

    [HttpPut("restock/id/quantity")]
    public IActionResult RestockProductQuantity(long id, int quantity)
    {
        var product = _policyWrap.Execute(() => dbContext.Products.FirstOrDefault(x => x.Id == id));
        if (product is null)
        {
            return BadRequest($"No product with Id {id} found. Please validate your input and try again.");
        }

        var previousQuantity = product.Quantity;
        product.Quantity += quantity;
        _policyWrap.Execute(() => dbContext.SaveChanges());
        return Ok($"{product.Name} stock increased from {previousQuantity} to {product.Quantity}.");
    }

    [HttpGet("all/active")]
    public IActionResult GetAllActiveProducts()
    {
        return _policyWrap.Execute(() => Ok(dbContext.Products
            .AsNoTracking()
            .Where(p => p.Active)
            .Include(p => p.Category)    //TODO category och company kommer tillbaks som null
            .Include(p => p.Company)    //TODO löste genom att byta till från system.data.entity till efcore
            .ToList()));

    }

    [HttpGet("all/inactive")]
    public IActionResult GetAllInactiveProducts()
    {
        return _policyWrap.Execute(() => Ok(dbContext.Products
            .Where(p => !p.Active)
            .ToList()));
    }


    [HttpPut("update/id")]
    public IActionResult UpdateProduct(long id, [FromBody] ProductUpdateDTO input)
    {
        
        var product = _policyWrap.Execute(() => dbContext.Products
            .FirstOrDefault(p => p.Id == id));
        if (product is null)
        {
            return BadRequest($"No product found with id {id}");
        }
        if (input.CategoryId is not null)
        {
            var category =
                _policyWrap.Execute(() => dbContext.Categories.FirstOrDefault(c => c.Id == input.CategoryId));
            if (category is null)
            {
                return BadRequest($"No category found with id {input.CategoryId}");
            }
            product.Category = category;
        }
        if (input.CompanyId is not null)
        {
            var company = _policyWrap.Execute(() => dbContext.Companies.FirstOrDefault(c => c.Id == input.CompanyId));
            if (company is null)
            {
                return BadRequest($"No company found with id {input.CompanyId}");
            }

            product.Company = company;
        }

        input.Adapt(product, ProductUpdateMapping.GetProductUpdateMappingConfig());
        _policyWrap.Execute(() => dbContext.SaveChanges());
        return Ok("Product updated.");
    }
}