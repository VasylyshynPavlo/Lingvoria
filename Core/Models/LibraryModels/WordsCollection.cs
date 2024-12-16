using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Models.LibraryModels;

public class WordsCollection
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [DataMember]
    public ObjectId Id { get; set; }

    public string? Title { get; set; }

    [Required] public string UserId { get; set; } = string.Empty;

    [Required] public string Language { get; set; } = string.Empty;

    public List<Word> Words { get; set; } = [];

    public object ToDto()
    {
        return new
        {
            Id = Id.ToString(),
            Title,
            Language,
            Words = Words.Select(w => w.ToDto()).ToList()
        };
    }
}