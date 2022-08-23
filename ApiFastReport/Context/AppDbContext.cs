using ApiFastReport.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiFastReport.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    { }

    public DbSet<Product>? Products { get; set; }
}
