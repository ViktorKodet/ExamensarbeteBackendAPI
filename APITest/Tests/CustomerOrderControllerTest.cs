using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Contexts;
using API.Controllers;
using API.Models;
using API.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace APITest.Tests;

public class CustomerOrderControllerTest
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

        var company1 = new Company() { Id = 1, Name = "Coca cola company" };
        var company2 = new Company() { Id = 2, Name = "Apple" };

        var category1 = new Category() { Id = 1, Name = "Läsk" };
        var category2 = new Category() { Id = 2, Name = "Elektronik" };

        var product1 = new Product()
        {
            Id = 1,
            Name = "Coca cola",
            Active = true,
            Quantity = 5,
            Company = company1,
            Category = category1,
            Picture = "xxx",
            Description = "cola",
            Price = 5
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
    public void ShouldAddOrder()
    {
        var dbContext = SetUpContext();
        var controller = new CustomerOrderController(dbContext);

        var customer = new CustomerCreationDTO()
        {
            FirstName = "Jens", 
            LastName = "Schmeid", 
            Adress = "Jensgatan 13", 
            City = "Uppsala",
            Email = "Jens.jens@gmail.com", 
            PhoneNumber = "1234567890", 
            ZipCode = "12345"
        };

        var productQuantities = new List<ProductQuantityDTO>()
        {
            new ProductQuantityDTO() { ProductId = 1, Quantity = 5 },
            new ProductQuantityDTO() { ProductId = 2, Quantity = 10 }
        };

        var output = controller.AddOrder(new CustomerOrderCreationDTO()
            { Customer = customer, Products = productQuantities });

        var order = dbContext.CustomerOrders.FirstOrDefault(c => c.Id == 1);
        Assert.Equal(1, order.Id);
        Assert.Equal("Jens", order.Customer.FirstName);
        Assert.Contains(order.ProductQuantities, p => p.Product.Id == 1);
        Assert.Contains(order.ProductQuantities, p => p.Quantity == 10);
        Assert.DoesNotContain(order.ProductQuantities, p => p.Quantity == 15);

        var customers = dbContext.Customers.ToList();
        Assert.Contains(customers, c => c.FirstName == "Jens");
        Assert.Contains(customers, c => c.LastName == "Schmeid");

        var cities = dbContext.Cities.ToList();
        Assert.Contains(cities, c => c.Name == "Uppsala");
    }
}