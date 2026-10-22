using OrigiBank.Application.Dtos;

namespace OrigiBank.Application.Services;

public interface ICustomerService
{
    Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
}