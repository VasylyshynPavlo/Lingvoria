using System.ComponentModel;

namespace Core.Models.Update;

public class UpdateWordCollectionModel
{
    [DefaultValue("")] public required string CollectionId { get; set; }
    [DefaultValue("")] public string? Language { get; set; }
    [DefaultValue("")] public string? Title { get; set; }
}