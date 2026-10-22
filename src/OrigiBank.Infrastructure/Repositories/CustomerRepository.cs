using Microsoft.EntityFrameworkCore;
using OrigiBank.Application.Abstractions;
using OrigiBank.Domain.Entities;
using OrigiBank.Infrastructure.Persistence;

namespace OrigiBank.Infrastructure.Repositories;

public sealed class CustomerRepository(OrigiBankDbContext context) : ICustomerRepository
{
    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default) =>
        await context.Customers.AddAsync(customer, cancellationToken);

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
}