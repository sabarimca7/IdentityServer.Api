namespace IdentityServer.Common.Exceptions;

public class BusinessLogicException : Exception
{
    public BusinessLogicException(string message) : base(message) { }
    public BusinessLogicException(string message, Exception innerException) : base(message, innerException) { }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
    public ValidationException(string message, Exception innerException) : base(message, innerException) { }
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
    public UnauthorizedException(string message, Exception innerException) : base(message, innerException) { }
}

public class InvalidGrantException : Exception
{
    public InvalidGrantException(string message) : base(message) { }
    public InvalidGrantException(string message, Exception innerException) : base(message, innerException) { }
}