using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Contexts;
using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace APITest.Tests;

public class CategoryControllerTest
{

    private HakimDbContext SetUpContext()
    {
        var options = new DbContextOptionsBuilder<HakimDbContext>()
            .UseInMemoryDatabase(databaseName: $"HakimDb{Guid.NewGuid()}")
            .Options;

        var dbContext = new HakimDbContext(options);

        Seed(dbContext);
        return dbContext;
    }
    private void Seed(HakimDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var category1 = new Category() { Id = 1, Name = "Läsk", Active = true};
        var category2 = new Category() { Id = 2, Name = "Elektronik" };

        context.Categories.Add(category1);
        context.Categories.Add(category2);

        context.SaveChanges();
    }

    [Fact]
    public void ShouldAddCategory()
    {
        var dbContext = SetUpContext();
        var controller = new CategoryController(dbContext);

        var output = controller.AddCategory("Kläder");

        var categories = dbContext.Categories.ToList();
        Assert.Contains(categories, c => c.Name == "Kläder");
    }

    [Fact]
    public void ShouldGetAllCategories()
    {
        var dbContext = SetUpContext();
        var controller = new CategoryController(dbContext);

        var result = (OkObjectResult)controller.GetAllCategories();
        var categories = (List<Category>)result.Value;

        Assert.Contains(categories, c => c.Id == 1);
        Assert.Contains(categories, c => c.Id == 2);
        Assert.Contains(categories, c => c.Name == "Läsk");
        Assert.Contains(categories, c => c.Name == "Elektronik");
    }

    [Fact]
    public void ShouldGetAllActiveCategories()
    {
        var dbContext = SetUpContext();
        var controller = new CategoryController(dbContext);

        var result = (OkObjectResult)controller.GetAllActiveCategories();
        var categories = (List<Category>)result.Value;

        Assert.Contains(categories, c => c.Id == 1);
        Assert.Contains(categories, c => c.Name == "Läsk");
    }

    [Fact]
    public void ShouldGetAllInactiveCategories()
    {
        var dbContext = SetUpContext();
        var controller = new CategoryController(dbContext);

        var result = (OkObjectResult)controller.GetAllInactiveCategories();
        var categories = (List<Category>)result.Value;

        Assert.Contains(categories, c => c.Id == 2);
        Assert.Contains(categories, c => c.Name == "Elektronik");
    }

    [Fact]
    public void ShouldToggleCategoryActivity()
    {
        var dbContext = SetUpContext();
        var controller = new CategoryController(dbContext);

        var output = controller.ToggleCategoryActive(2);

        var categories = dbContext.Categories.Where(c => c.Active == true);

        Assert.Contains(categories, c => c.Id == 2);
    }

    [Fact]
    public void ShouldUpdateCategory()
    {
        var dbContext = SetUpContext();
        var controller = new CategoryController(dbContext);

        var output = controller.UpdateCategory(1, "Kläder");

        var categories = dbContext.Categories.ToList();
        Assert.Contains(categories, c => c.Name == "Kläder");
    }
}