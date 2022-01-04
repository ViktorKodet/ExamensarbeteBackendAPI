using System.Data.Entity;
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
public class CompanyController : ControllerBase
{

    private readonly HakimDbContext dbContext;
    private readonly PolicyWrap _policyWrap;

    public CompanyController(HakimDbContext dbContext)
    {
        this.dbContext = dbContext;
        var retry = Retry.GetPolicy();
        var timeout = Timeout.GetPolicy();
        _policyWrap = Policy.Wrap(retry, timeout);
    }

    // POST api/<CompanyController>
    [HttpPost("name")]
    public IActionResult AddCompany(string name)
    {
        var company = _policyWrap.Execute(() => dbContext.Companies.FirstOrDefault(x => x.Name == name));
        if (company is not null)
        {
            return Ok($"A Company by the name {name} already exists in the database. Id : {company.Id}");
        }
        _policyWrap.Execute(() => dbContext.Companies.Add(new Company { Name = name }));
        _policyWrap.Execute(() => dbContext.SaveChanges());
        return Ok("Company added.");
    }
    [HttpGet]
    public IActionResult GetAllCompanies()
    {
        return _policyWrap.Execute(() => Ok(dbContext.Companies.ToList()));
    }


    [HttpPut]
    public IActionResult UpdateCompany([FromBody] Company c)
    {
        var company = _policyWrap.Execute(() => dbContext.Companies.FirstOrDefault(x => x.Id == c.Id));
        if (company is null)
        {
            return BadRequest($"No company with Id {c.Id} found. Please validate your input and try again.");
        }

        var oldName = company.Name;
        company.Name = c.Name;
        _policyWrap.Execute(() => dbContext.SaveChanges());
        return Ok($"{oldName} successfully updated to {c.Name}.");
    }


}