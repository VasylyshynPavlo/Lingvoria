using System.ComponentModel;

namespace Core.Models.UserModels;

public class LoginForm
{
    [DefaultValue("")] public required string UsernameOrEmail { get; set; }
    [DefaultValue("")] public required string Password { get; set; }
}