using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Models;

public class Word
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [DataMember]
    public ObjectId Id { get; set; }

    [Required]
    public string WordText { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Required]
    public string Translate { get; set; } = string.Empty;
    public List<Example> Examples { get; set; }
}