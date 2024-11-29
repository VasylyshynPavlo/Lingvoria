using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entities;

public class WordsCollectionModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    public string? Title { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public List<WordModel> Words { get; set; }
}