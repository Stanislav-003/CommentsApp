namespace backend.Shared.Errors;

public class AppException : Exception
{
    public AppException(string message) : base(message)
    {
    }
}
