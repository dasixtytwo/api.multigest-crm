namespace DA.Multigest.API.Exceptions;

public class NotFoundException: Exception
{
    public NotFoundException(string message): base(message) {}
}
