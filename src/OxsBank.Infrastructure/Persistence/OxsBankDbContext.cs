using Microsoft.EntityFrameworkCore;
using OxsBank.Domain.Entities;

namespace OxsBank.Infrastructure.Persistence;

public class OxsBankDbContext(DbContextOptions<OxsBankDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Account>()
            .HasIndex(a => a.Cnpj)
            .IsUnique(); // Garante que um CNPJ não seja cadastrado duas vezes

        builder.Entity<Transaction>()
            .HasOne(t => t.Account)
            .WithMany()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}