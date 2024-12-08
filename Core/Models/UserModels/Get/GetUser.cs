namespace Core.Models.UserModels.Get;

public class GetUser
{
    public required string Id { get; set; }
    public string? Avatar { get; set; }
    public required string Username { get; set; }
    public string? FullName { get; set; }
}