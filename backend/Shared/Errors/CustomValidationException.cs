using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend.Shared.Errors;

public class CustomValidationException : Exception
{
    public List<Error> Errors { get; }

    public CustomValidationException(string message, List<Error> errors) : base(message)
    {
        Errors = errors;
    }
}