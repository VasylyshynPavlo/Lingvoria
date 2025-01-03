namespace Core.Models;

public class Response
{
    public Response(int code, string message, object? data = null)
    {
        Code = code;
        Message = message;
        Data = data;
    }

    public int Code { get; set; }
    public string? Message { get; set; }

    public object? Data { get; set; }
}