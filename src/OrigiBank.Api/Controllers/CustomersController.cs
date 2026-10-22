using Microsoft.AspNetCore.Mvc;
using OrigiBank.Application.Dtos;
using OrigiBank.Application.Services;

namespace OrigiBank.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController(ICustomerService service) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CustomerDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await service.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, customer);
    }
}