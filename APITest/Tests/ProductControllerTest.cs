using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Contexts;
using API.Controllers;
using API.Models;
using API.Models.DTOs;
using APITest.TestModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace APITest.Tests;

public class ProductControllerTest
{
    private HakimDbContext SetUpContext()
    {
        var options = new DbContextOptionsBuilder<HakimDbContext>()
            .UseInMemoryDatabase(databaseName: "HakimDb")
            .Options;

        var dbContext = new HakimDbContext(options);
        //var dbContext = A.Fake<HakimDbContext>();

        Seed(dbContext);
        return dbContext;
    }
    private void Seed(HakimDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var company1 = new Company() { Id = 1, Name = "Coca cola company" };
        var company2 = new Company() { Id = 2, Name = "Apple" };

        var category1 = new Category() { Id = 1, Name = "Läsk" };
        var category2 = new Category() { Id = 2, Name = "Elektronik" };

        var product1 = new Product()
        {
            Id = 1, Name = "Coca cola", Active = true, Quantity = 5, Company = company1, Category = category1, Picture = "xxx",
            Description = "cola", Price = 5
        };
        var product2 = new Product()
        {
            Id = 2,
            Name = "IPhone",
            Active = false,
            Quantity = 4,
            Company = company2,
            Category = category2,
            Picture = "xxx",
            Description = "phone",
            Price = 5000
        };

        context.Companies.Add(company1);
        context.Companies.Add(company2);

        context.Categories.Add(category1);
        context.Categories.Add(category2);

        context.Products.Add(product1);
        context.Products.Add(product2);

        context.SaveChanges();
    }

    [Fact]
    public void ShouldReturnAllActiveProducts()
    {


        var dbContext = SetUpContext();
        var controller = new ProductController(dbContext);

        var products = (OkObjectResult)controller.GetAllActiveProducts();
        var result = (List<Product>)products.Value;
        Assert.Equal(1, result.Count);
        Assert.Equal("Coca cola", result[0].Name);


    }

    [Fact]
    public void ShouldReturnAllInactiveProducts()
    {


        var dbContext = SetUpContext();
        var controller = new ProductController(dbContext);

        var products = (OkObjectResult)controller.GetAllInactiveProducts();
        var result = (List<Product>)products.Value;
        Assert.Equal(1, result.Count);
        Assert.Equal("IPhone", result[0].Name);

    }

    [Fact]
    public void ShouldUpdateProducts()
    {
        var dbContext = SetUpContext();
        var controller = new ProductController(dbContext);

        var products = (OkObjectResult)controller.GetAllActiveProducts();
        var result = (List<Product>)products.Value;
        Assert.Equal("Coca cola", result[0].Name);

        var output = controller.UpdateProduct(1, new ProductUpdateDTO() { Name = "Coca cola zero" });
        dbContext.SaveChanges();

        products = (OkObjectResult)controller.GetAllActiveProducts();
        result = (List<Product>)products.Value;
        Assert.Equal("Coca cola zero", result[0].Name);

    }
    [Fact]
    public void ShouldAddProduct()
    {
        var dbContext = SetUpContext();
        var controller = new ProductController(dbContext);

        var output = controller.AddProduct(new ProductCreationDTO()
        {
            CategoryId = 1,
            CompanyId = 1,
            Description = "Beverage",
            Name = "Coca cola cherry",
            Picture = "xxx",
            Price = 12.99
        });
        dbContext.SaveChanges();

        var products = (OkObjectResult)controller.GetAllInactiveProducts();
        var result = (List<Product>)products.Value;
        Assert.Equal("Coca cola cherry", result[1].Name);

    }

    [Fact]
    public void ShouldToggleProductActivity()
    {
        var dbContext = SetUpContext();
        var controller = new ProductController(dbContext);

        var output = controller.ToggleProductActive(1);
        output = controller.ToggleProductActive(2);
        dbContext.SaveChanges();

        var products = (OkObjectResult)controller.GetAllActiveProducts();
        var result = (List<Product>)products.Value;
        Assert.Equal("IPhone", result[0].Name);

        products = (OkObjectResult)controller.GetAllInactiveProducts();
        result = (List<Product>)products.Value;
        Assert.Equal("Coca cola", result[0].Name);
    }

    [Fact]
    public void ShouldIncreaseProductQuantity()
    {
        var dbContext = SetUpContext();
        var controller = new ProductController(dbContext);

        var output = controller.RestockProductQuantity(1, 5);
        dbContext.SaveChanges();

        var products = (OkObjectResult)controller.GetAllActiveProducts();
        var result = (List<Product>)products.Value;
        Assert.Equal(10, result[0].Quantity);
    }
}