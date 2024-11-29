using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Models;  

public class UserModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [DataMember]
    public ObjectId Id { get; set; }
    public string? AvatarUrl { get; set; } = string.Empty; 
    public string Username { get; set; } = string.Empty;
    [DataMember]
    public string NormalizedUsername { get; set; } = string.Empty; 
    public string? FullName { get; set; } = string.Empty; 
    public string Email { get; set; } = string.Empty;
    [DataMember]
    public string NormalizedEmail { get; set; } = string.Empty; 
    public bool EmailConfirmed { get; set; } = false;
    [DataMember]
    public string PasswordHash { get; set; } = string.Empty;
    public bool TwoFactorEnabled { get; set; } = false; 
    [DataMember]
    public bool LockoutEnabled { get; set; } = false;
    [DataMember]
    public DateTimeOffset? LockoutEnd { get; set; } = null;
    public string? PhoneNumber { get; set; } = string.Empty;
    [DataMember]
    public bool PhoneNumberConfirmed { get; set; } = false;
    [DataMember]
    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();
    [DataMember]
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    [DataMember]
    public string? Provider { get; set; }
    [DataMember]
    public string? ProviderUserId { get; set; }

    public override string ToString()
    {
        return Username;
    }
}