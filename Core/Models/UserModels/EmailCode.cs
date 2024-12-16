using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Models.UserModels;

public class EmailCode
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    public string UserId { get; set; }

    public string Code { get; set; }

    public DateTime ValidUntil { get; set; }

    public EmailCode(string userId, string code)
    {
        Id = ObjectId.GenerateNewId();
        UserId = userId;
        Code = code;
        ValidUntil = DateTime.Now.AddMinutes(30);
    }
}