using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrigiBank.Domain.Entities;

namespace OrigiBank.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();
        builder.Property(c => c.FullName).IsRequired().HasMaxLength(150);
        builder.Property(c => c.DateOfBirth).IsRequired();
        builder.Property(c => c.Gender).IsRequired();
        builder.Property(c => c.MonthlyIncome).HasPrecision(18, 2);
    }
}