using AzerothConnect.Database.Auth;

using Microsoft.EntityFrameworkCore;

namespace AzerothConnect.Database.Contexts;

public class AuthDbContext : DbContext
{
    public DbSet<Account> Account { get; set; }
    public DbSet<AccountAccess> AccountAccess { get; set; }

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().ToTable("account", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<AccountAccess>().ToTable("account_access", t => t.ExcludeFromMigrations());
    }
}
