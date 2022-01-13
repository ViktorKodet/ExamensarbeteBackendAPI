using API.Contexts;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Wrap;
using API.ResiliencePolicies;
using Timeout = API.ResiliencePolicies.Timeout;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{

    private readonly HakimDbContext dbContext;
    private readonly PolicyWrap _policyWrap;

    public CategoryController(HakimDbContext dbContext)
    {
        this.dbContext = dbContext;
        var retry = Retry.GetPolicy();
        var timeout = Timeout.GetPolicy();
        _policyWrap = Policy.Wrap(retry, timeout);
    }

    [HttpPost("name")]
    public IActionResult AddCategory(string name)
    {
        var category = _policyWrap.Execute(() => dbContext.Categories.FirstOrDefault(x => x.Name == name));
        if (category is not null)
        {
            return Ok($"A Category by the name {name} already exists in the database. Id : {category.Id}");
        }
        _policyWrap.Execute(() => dbContext.Categories.Add(new Category { Name = name }));
        _policyWrap.Execute(() => dbContext.SaveChanges());
        return Ok("Category added.");
    }

    [HttpGet("all")]
    public IActionResult GetAllCategories()
    {
        return _policyWrap.Execute(() => Ok(dbContext.Categories.ToList()));
    }

    [HttpGet("all/active")]
    public IActionResult GetAllActiveCategories()
    {
        return _policyWrap.Execute(() => Ok(dbContext.Categories.Where(c => c.Active).ToList()));
    }

    [HttpGet("all/inactive")]
    public IActionResult GetAllInactiveCategories()
    {
        return _policyWrap.Execute(() => Ok(dbContext.Categories.Where(c => !c.Active).ToList()));
    }

    [HttpPut("toggle/id")]
    public IActionResult ToggleCategoryActive(long id)
    {
        var category = _policyWrap.Execute(() => dbContext.Categories.FirstOrDefault(x => x.Id == id));
        if (category is null)
        {
            return BadRequest($"No category with Id {id} found. Please validate your input and try again.");
        }

        category.Active = !category.Active;
        _policyWrap.Execute(() => dbContext.SaveChanges());
        return Ok($"{category.Name} active-status successfully updated to {category.Active}.");
    }

    [HttpPut("update/id/name")]
    public IActionResult UpdateCategory(long id, string name)
    {
        var category = _policyWrap.Execute(() => dbContext.Categories.FirstOrDefault(x => x.Id == id));
        if (category is null)
        {
            return BadRequest($"No category with Id {id} found. Please validate your input and try again.");
        }

        var oldName = category.Name;
        category.Name = name;
        _policyWrap.Execute(() => dbContext.SaveChanges());
        return Ok($"{oldName} successfully updated to {name}.");
    }
}