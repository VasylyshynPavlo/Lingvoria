namespace Data.Models;

public class Word
{
    public string WordText { get; set; }
    public string Description { get; set; }
    public string Translate { get; set; }
    public List<Example> Examples { get; set; }
}