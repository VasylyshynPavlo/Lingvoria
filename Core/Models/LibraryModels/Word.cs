using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Models.LibraryModels;

public class Word
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [DataMember]
    public ObjectId Id { get; set; }

    public string Text { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;

    public string? Translate { get; set; } = string.Empty;
    public List<Example> Examples { get; set; } = [];

    public object ToDto()
    {
        return new
        {
            Id = Id.ToString(),
            Text,
            Translate,
            Description,
            Examples = Examples.Select(e => e.ToDto()).ToList()
        };
    }
}