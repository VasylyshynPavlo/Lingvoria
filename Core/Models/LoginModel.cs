using System.ComponentModel;

namespace Core.Models
{
    public class LoginModel
    {
        [DefaultValue("")] public string? Username { get; set; }
        [DefaultValue("")] public string? Email { get; set; }
        [DefaultValue("")] public required string Password { get; set; }
    }
}