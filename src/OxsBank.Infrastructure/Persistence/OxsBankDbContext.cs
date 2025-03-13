using Microsoft.EntityFrameworkCore;
using OxsBank.Domain.Entities;

namespace OxsBank.Infrastructure.Persistence;

public class OxsBankDbContext : DbContext
{
    public OxsBankDbContext(DbContextOptions<OxsBankDbContext> options) : base(options)
    {
        
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}