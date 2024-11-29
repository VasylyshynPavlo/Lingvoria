using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class CreateWordModel
{
    [DefaultValue("Empty")] public required string Word { get; set; }
    [DefaultValue("null")] public required string CollectionId { get; set; }
    [DefaultValue("Empty")] public required string Description { get; set; }
    [DefaultValue("")] public string? Translate { get; set; }
}