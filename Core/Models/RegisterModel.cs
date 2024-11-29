using System.ComponentModel;

namespace Core.Models;

public class RegisterModel
{
    public string? AvatarUrl { get; set; }
    [DefaultValue("")]public required string Username { get; set; }
    public string? FullName { get; set; } 
    [DefaultValue("example@mail.com")]public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string Password { get; set; }
}