using System.Data.Entity;
using API.Contexts;
using API.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {

        private readonly HakimDbContext dbContext;

        public CompanyController(HakimDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // POST api/<CompanyController>
        [HttpPost]
        public IActionResult AddCompany([FromBody] string name)
        {
            var company = dbContext.Companies.FirstOrDefault(x => x.Name == name);
            if (company is not null)
            {
                return Ok($"A Company by the name {name} already exists in the database. Id : {company.Id}");
            }
            dbContext.Companies.Add(new Company { Name = name });
            dbContext.SaveChanges();
            return Ok("Company added.");
        }
        [HttpGet]
        public IActionResult GetAllCompanies()
        {
            return Ok(dbContext.Companies.ToList());
        }


        [HttpPut]
        public IActionResult UpdateCompany([FromBody] Company c)
        {
            var company = dbContext.Companies.FirstOrDefault(x => x.Id == c.Id);
            if (company is null)
            {
                return BadRequest($"No company with Id {c.Id} found. Please validate your input and try again.");
            }

            var oldName = company.Name;
            company.Name = c.Name;
            dbContext.SaveChanges();
            return Ok($"{oldName} successfully updated to {c.Name}.");
        }


    }
}
