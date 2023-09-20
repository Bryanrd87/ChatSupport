using ChatSupport.Domain;
using Microsoft.EntityFrameworkCore;

namespace ChatSupport.Persistence;
public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<ChatSession> ChatSessions { get; set; }
    public DbSet<Agent> Agents { get; set; }
    public DbSet<ChatMessage> Messages { get; set; }

    public ApplicationDbContext(){}

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {        
    }
}

