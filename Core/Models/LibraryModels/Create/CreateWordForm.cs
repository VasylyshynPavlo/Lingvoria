using System.ComponentModel;

namespace Core.Models.LibraryModels.Create;

public class CreateWordForm
{
    [DefaultValue("")] public required string Text { get; set; }
    [DefaultValue("")] public string? Description { get; set; }
    [DefaultValue("")] public string? Translate { get; set; }
}