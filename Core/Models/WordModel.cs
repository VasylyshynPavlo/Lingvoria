using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Models;

public class WordModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [DataMember]
    public ObjectId Id { get; set; }
    public string Word { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public string? Translate { get; set; } = string.Empty;
    public List<ExampleModel> Examples { get; set; }
}