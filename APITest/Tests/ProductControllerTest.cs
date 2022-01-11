using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Contexts;
using API.Controllers;
using API.Models;
using APITest.TestModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace APITest.Tests;

public class ProductControllerTest
{

    private void Seed(HakimDbContext context)
    {
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


        var options = new DbContextOptionsBuilder<HakimDbContext>()
            .UseInMemoryDatabase(databaseName: "HakimDb")
            .Options;
        var dbContext = new HakimDbContext(options);
        
            Seed(dbContext);      //TODO funkar inte när man kör alla tester samtidigt, de använder typ samma in memory db. Måste sopa dem mellan eller något
            var controller = new ProductController(dbContext);

            var products = (OkObjectResult)controller.GetAllActiveProducts();
            var result = (List<Product>)products.Value;
            Assert.Equal(1, result.Count);
            Assert.Equal("Coca cola", result[0].Name);

            dbContext.Database.EnsureDeleted();

    }

    [Fact]
    public void ShouldReturnAllInactiveProducts()
    {


        var options = new DbContextOptionsBuilder<HakimDbContext>()
            .UseInMemoryDatabase(databaseName: "HakimDb")
            .Options;

        var dbContext = new HakimDbContext(options);
        
            Seed(dbContext);
            var controller = new ProductController(dbContext);

            var products = (OkObjectResult)controller.GetAllInactiveProducts();
            var result = (List<Product>)products.Value;
            Assert.Equal(1, result.Count);
            Assert.Equal("IPhone", result[0].Name);

        dbContext.Database.EnsureDeleted();
    }
}