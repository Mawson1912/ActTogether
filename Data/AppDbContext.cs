using Microsoft.EntityFrameworkCore;
using ActTogether.Models;

namespace ActTogether.Data
{
    // EF Core database context — maps models to SQL Server tables
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<DismissedPost> DismissedPosts { get; set; }
    }
}
