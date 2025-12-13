namespace frontend.Errors;

public class Error
{
    public Error()
    {
    }

    public Error(string details)
    {
        Detail = details;
    }

    public string Type { get; set; }
    public string Title { get; set; }
    public int Status { get; set; }
    public string Detail { get; set; }
    public string? TraceId { get; set; }
}