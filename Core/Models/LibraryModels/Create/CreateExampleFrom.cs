using System.ComponentModel;

namespace Core.Models.LibraryModels.Create;

public class CreateExampleFrom
{
    [DefaultValue("Empty")] public required string Text { get; set; } = string.Empty;
    [DefaultValue("")] public string? Translate { get; set; } = string.Empty;
}