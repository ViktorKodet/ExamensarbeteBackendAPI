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

public class CompanyControllerTest
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

        context.Companies.Add(company1);
        context.Companies.Add(company2);

        context.SaveChanges();
    }

    [Fact]
    public void ShouldAddCompany()
    {
        var dbContext = SetUpContext();
        var controller = new CompanyController(dbContext);

        var output = controller.AddCompany("Rusta");
        var companies = dbContext.Companies.ToList();

        Assert.Contains(companies, c => c.Name == "Rusta");
    }

    [Fact]
    public void ShouldGetAllCompanies()
    {
        var dbContext = SetUpContext();
        var controller = new CompanyController(dbContext);

        var result = (OkObjectResult)controller.GetAllCompanies();
        var companies = (List<Company>)result.Value;

        Assert.Equal(2, companies.Count);
        Assert.Contains(companies, c => c.Id == 1);
        Assert.Contains(companies, c => c.Id == 2);
        Assert.Contains(companies, c => c.Name == "Coca cola company");
        Assert.Contains(companies, c => c.Name == "Apple");
    }

    [Fact]
    public void ShouldUpdateCompany()
    {
        var dbContext = SetUpContext();
        var controller = new CompanyController(dbContext);

        var output = controller.UpdateCompany(1, "Rusta");

        var companies = dbContext.Companies.ToList();

        Assert.Contains(companies, c => c.Name == "Rusta");
    }
}