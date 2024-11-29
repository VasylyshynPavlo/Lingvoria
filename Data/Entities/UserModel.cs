using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entities;  

public class UserModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    public string? AvatarUrl { get; set; } = string.Empty; 
    public string Username { get; set; } = string.Empty;
    public string NormalizedUsername { get; set; } = string.Empty; 
    public string? FullName { get; set; } = string.Empty; 
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty; 
    public bool EmailConfirmed { get; set; } = false;
    public string PasswordHash { get; set; } = string.Empty;
    public bool TwoFactorEnabled { get; set; } = false; 
    public bool LockoutEnabled { get; set; } = false;
    public DateTimeOffset? LockoutEnd { get; set; } = null;
    public string? PhoneNumber { get; set; } = string.Empty;
    public bool PhoneNumberConfirmed { get; set; } = false;
    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    public string? Provider { get; set; }
    public string? ProviderUserId { get; set; }

    public override string ToString()
    {
        return Username;
    }
}