namespace Core.Models;

public class Response
{
    public int Code { get; set; }
    public string? Message { get; set; }

    public object? Data { get; set; }
    public Response(int code, string message, object? data = null)
    {
        this.Code = code;
        this.Message = message;
        this.Data = data;
    }
}