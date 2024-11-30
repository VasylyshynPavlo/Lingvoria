
using System.ComponentModel;

namespace Core.Models.Create;

public class CreateWordsCollectionModel
{
    [DefaultValue("null")] public required string UserId { get; set; } 
    [DefaultValue("English")]public required string Language { get; set; }
    [DefaultValue("")]public string? Title { get; set; }
}