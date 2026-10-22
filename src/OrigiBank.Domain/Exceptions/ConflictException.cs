namespace OrigiBank.Domain.Exceptions;

public sealed class ConflictException(string message) : BusinessException(message);