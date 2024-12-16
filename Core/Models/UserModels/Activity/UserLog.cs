using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Models.UserModels.Activity;

public class UserLog(string action, string? context = null)
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    public string Action { get; set; } = action;
    public string? Context { get; set; } = context;
    public DateTime Date { get; set; } = DateTime.Now;
}