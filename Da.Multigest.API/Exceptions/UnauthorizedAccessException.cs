namespace DA.Multigest.API.Exceptions;

public class UnauthorizedAccessException: Exception
{
    public UnauthorizedAccessException(string message) : base(message) {}
}
