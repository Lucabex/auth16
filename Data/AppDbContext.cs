using auth16.Models;
using Microsoft.EntityFrameworkCore;

namespace auth16.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    public DbSet<User> User{get;set;}
}