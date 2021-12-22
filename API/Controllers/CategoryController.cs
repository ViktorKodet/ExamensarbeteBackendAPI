using API.Contexts;
using API.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{

    private readonly HakimDbContext dbContext;

    public CategoryController(HakimDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpPost("name")]
    public IActionResult AddCategory(string name)
    {
        var category = dbContext.Categories.FirstOrDefault(x => x.Name == name);
        if (category is not null)
        {
            return Ok($"A Category by the name {name} already exists in the database. Id : {category.Id}");
        }
        dbContext.Categories.Add(new Category { Name = name });
        dbContext.SaveChanges();
        return Ok("Category added.");
    }

    [HttpGet("all")]
    public IActionResult GetAllCategories()
    {
        return Ok(dbContext.Categories.ToList());
    }

    [HttpGet("all/active")]
    public IActionResult GetAllActiveCategories()
    {
        return Ok(dbContext.Categories.Where(c => c.Active).ToList());
    }

    [HttpGet("all/inactive")]
    public IActionResult GetAllInactiveCategories()
    {
        return Ok(dbContext.Categories.Where(c => !c.Active).ToList());
    }

    [HttpPut("toggle/id")]
    public IActionResult ToggleCategoryActive(long id)
    {
        var category = dbContext.Categories.FirstOrDefault(x => x.Id == id);
        if (category is null)
        {
            return BadRequest($"No category with Id {id} found. Please validate your input and try again.");
        }

        category.Active = !category.Active;
        dbContext.SaveChanges();
        return Ok($"{category.Name} active-status successfully updated to {category.Active}.");
    }

    [HttpPut]
    public IActionResult UpdateCategory([FromBody] Category c)
    {
        var category = dbContext.Categories.FirstOrDefault(x => x.Id == c.Id);
        if (category is null)
        {
            return BadRequest($"No category with Id {c.Id} found. Please validate your input and try again.");
        }

        var oldName = category.Name;
        category.Name = c.Name;
        dbContext.SaveChanges();
        return Ok($"{oldName} successfully updated to {c.Name}.");
    }
}