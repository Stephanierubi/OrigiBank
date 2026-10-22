namespace OrigiBank.Domain.Exceptions;

public sealed class NotFoundException : BusinessException
{
    public string? ResourceName { get; }
    public string? ResourceKey { get; }

    // Constructor con mensaje libre
    public NotFoundException(string message)
        : base(message)
    {
    }

    // Constructor estructurado (cuentas, clientes, etc.)
    public NotFoundException(string resourceName, string resourceKey)
        : base($"No se encontró el recurso '{resourceName}' con el identificador '{resourceKey}'.")
    {
        ResourceName = resourceName;
        ResourceKey = resourceKey;
    }
}