using OrigiBank.Application.Abstractions;
using OrigiBank.Application.Dtos;
using OrigiBank.Domain.Entities;

namespace OrigiBank.Application.Services;

public sealed class CustomerService(
    ICustomerRepository customers,
    IUnitOfWork unitOfWork,
    TimeProvider clock) : ICustomerService
{
    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);

        var customer = new Customer(
            request.FullName,
            request.DateOfBirth,
            request.Gender,
            request.MonthlyIncome,
            today);

        await customers.AddAsync(customer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.ToDto();
    }
}