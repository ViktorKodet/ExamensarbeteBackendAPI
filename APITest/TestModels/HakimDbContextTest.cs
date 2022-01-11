using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Contexts;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace APITest.TestModels;

public class HakimDbContextTest : HakimDbContext
{
    public HakimDbContextTest(DbContextOptions<HakimDbContext> options) : base(options)
    {
    }
}