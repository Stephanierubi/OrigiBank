using Microsoft.EntityFrameworkCore;
using OrigiBank.Domain.Entities;

namespace OrigiBank.Infrastructure.Persistence;

public sealed class OrigiBankDbContext(DbContextOptions<OrigiBankDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrigiBankDbContext).Assembly);
    }
}