namespace Data.Models;

public class WordsCollection
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Language { get; set; }
    public List<Word> Words { get; set; }
}