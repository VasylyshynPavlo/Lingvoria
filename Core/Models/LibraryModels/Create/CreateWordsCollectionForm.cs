using System.ComponentModel;

namespace Core.Models.LibraryModels.Create;

public class CreateWordsCollectionForm
{
    [DefaultValue("English")]public required string Language { get; set; }
    [DefaultValue("")]public string? Title { get; set; }
}