using System.ComponentModel;

namespace Core.Models.LibraryModels.Create;

public class CreateWordForm
{
    [DefaultValue("Empty")] public required string Text { get; set; }
    [DefaultValue("Empty")] public required string Description { get; set; }
    [DefaultValue("")] public string? Translate { get; set; }
}