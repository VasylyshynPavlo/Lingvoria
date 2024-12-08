using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Models.LibraryModels;

public class Example
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [DataMember]
    public ObjectId Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Translate { get; set; } = string.Empty;
    
    public object ToDto()
    {
        return new
        {
            Id = Id.ToString(),
            Text,
            Translate
        };
    }

}