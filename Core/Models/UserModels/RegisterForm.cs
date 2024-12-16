using System.ComponentModel;

namespace Core.Models.UserModels;

public class RegisterForm
{
    [DefaultValue("")] public string? AvatarUrl { get; set; }
    [DefaultValue("")] public required string Username { get; set; }
    [DefaultValue("")] public string? FullName { get; set; }
    [DefaultValue("example@mail.com")] public required string Email { get; set; }
    [DefaultValue("")] public string? PhoneNumber { get; set; }
    [DefaultValue("")] public required string Password { get; set; }
}