using Microsoft.EntityFrameworkCore;
using OrigiBank.Application.Abstractions;
using OrigiBank.Domain.Exceptions;

namespace OrigiBank.Infrastructure.Persistence;

public sealed class UnitOfWork(OrigiBankDbContext context) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException("La cuenta fue modificada por otra operación. Intente nuevamente.");
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new ConflictException("El registro ya existe (valor duplicado).");
        }
    }
}