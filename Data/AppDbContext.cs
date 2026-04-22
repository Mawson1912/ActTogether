using Microsoft.EntityFrameworkCore;
using ActTogether.Models;

namespace ActTogether.Data
{
    // EF Core database context — maps models to SQL Server tables
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
    }
}
