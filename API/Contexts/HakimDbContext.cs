using System.Data.Common;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Contexts;

public class HakimDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerOrder> CustomerOrders { get; set; }
    public DbSet<OutOfStock> OutOfStocks { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductQuantity> ProductQuantities { get; set; }

    public HakimDbContext(DbContextOptions<HakimDbContext> options) : base(options)
    {
        
    }


}