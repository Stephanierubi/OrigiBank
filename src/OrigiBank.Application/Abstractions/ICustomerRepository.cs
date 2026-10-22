using OrigiBank.Domain.Entities;

namespace OrigiBank.Application.Abstractions;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}