using System.ComponentModel;

namespace Core.Models;

public class CreateExampleModel
{
    [DefaultValue("null")] public required string collectionId { get; set; }
    [DefaultValue("null")] public required string wordId { get; set; }
    [DefaultValue("Empty")] public required string Text { get; set; } = string.Empty;
    [DefaultValue("")] public string? Translate { get; set; } = string.Empty;
}