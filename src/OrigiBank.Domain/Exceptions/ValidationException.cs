namespace OrigiBank.Domain.Exceptions;

public sealed class ValidationException(string message) : BusinessException(message);