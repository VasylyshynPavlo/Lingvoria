namespace Core.Models;

public class Result
{
    public string? Code { get; set; }
    public string? Message { get; set; }

    public object? Data { get; set; }
    public Result(string code, string message, object? data = null)
    {
        this.Code = code;
        this.Message = message;
        this.Data = data;
    }

    public Result()
    {
    }

}