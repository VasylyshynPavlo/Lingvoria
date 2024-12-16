using AutoMapper;
using Core.Interfaces;
using Core.Models;
using Core.Models.LibraryModels;
using Core.Models.LibraryModels.Create;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Core.Services;

public class WordService(LingvoriaDbContext context, IMapper mapper) : IWordService
{
    #region Logic

    public async Task<bool> IsMyCollection(string collectionId, string userId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        if (collection == null) return false;
        if (collection.UserId != userId) return false;
        return true;
    }

    #endregion

    //Collections-----------------------------------

    #region Collections

    public async Task<Response> CreateCollection(CreateWordsCollectionForm collectionModel, string userId)
    {
        var newCollection = new WordsCollection
        {
            Id = ObjectId.GenerateNewId(),
            UserId = userId,
            Language = collectionModel.Language,
            Title = collectionModel.Title,
            Words = []
        };
        await context.WordsCollections.InsertOneAsync(newCollection);

        return new Response(200, "Collection Created");
    }

    public async Task<Response> DeleteCollection(string collectionId, string userId)
    {
        var collection =
            await context.WordsCollections.Find(c => c.Id.ToString() == collectionId).FirstOrDefaultAsync();
        if (collection == null) return new Response(404, "Collection not found");
        if (collection.UserId != userId) return new Response(403, "You are not the owner of this collection");
        var result = await context.WordsCollections.DeleteOneAsync(w => w.Id == new ObjectId(collectionId));
        return new Response(200, "Collection Deleted");
    }

    public async Task<Response> GetCollections(string userId)
    {
        var collections = await context.WordsCollections.Find(_ => true).ToListAsync();
        if (collections == null || collections.Count == 0) return new Response(404, "Collections Not Found");
        List<WordsCollection> collectionToReturn = [];
        collectionToReturn.AddRange(collections.Where(collection => collection.UserId == userId));
        return collectionToReturn.Count == 0
            ? new Response(404, "You don't have any collections")
            : new Response(200, "Collections Retrieved", collectionToReturn);
    }

    public async Task<Response> UpdateCollection(string collectionId, string? language, string? title, string userId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        if (collection == null) return new Response(404, "Collection Not Found");
        if (collection.UserId != userId) return new Response(403, "You are not the owner of this collection");
        if (language != null) collection.Language = language;
        if (title != null) collection.Title = title;

        var result =
            await context.WordsCollections.ReplaceOneAsync(w => w.Id == new ObjectId(collectionId), collection);
        return new Response(200, "Collection Updated", collection);
    }

    public async Task<Response> GetCollectionById(string collectionId, string userId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        if (collection == null) return new Response(404, "Collection Not Found");
        if (collection.UserId != userId) return new Response(403, "You are not the owner of this collection");
        return new Response(200, "Collection Retrieved", collection);
    }

    #endregion

    //Words-----------------------------------

    #region Words

    public async Task<Response> AddWord(CreateWordForm word, string collectionId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        var newWord = new Word
        {
            Id = ObjectId.GenerateNewId(),
            Text = word.Text,
            Translate = word.Translate,
            Description = word.Description,
            Examples = new List<Example>()
        };
        collection.Words.Add(newWord);
        var result =
            await context.WordsCollections.ReplaceOneAsync(w => w.Id == new ObjectId(collectionId), collection);
        return result.ModifiedCount == 0
            ? new Response(404, "Collection Not Found")
            : new Response(200, "Word Created");
    }

    public async Task<Response> AddWords(List<CreateWordForm> words, string collectionId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        if (collection == null) return new Response(404, "Collection Not Found");
        List<Word> newWords = [];
        Console.WriteLine(words.Count);
        foreach (var word in words)
        {
            var newWord = new Word
            {
                Id = ObjectId.GenerateNewId(),
                Text = word.Text,
                Translate = word.Translate,
                Description = word.Description,
                Examples = []
            };
            newWords.Add(newWord);
        }

        foreach (var word in newWords)
        {
            collection.Words.Add(word);
            Console.WriteLine(word);
        }

        var result =
            await context.WordsCollections.ReplaceOneAsync(w => w.Id == new ObjectId(collectionId), collection);

        return new Response(200, "Word Created");
    }

    public async Task<Response> DeleteWord(string collectionId, string wordId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        var wordToRemove = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));
        if (wordToRemove == null) return new Response(404, "Word Not Found");
        collection.Words.Remove(wordToRemove);
        await context.WordsCollections.ReplaceOneAsync(w => w.Id == new ObjectId(collectionId), collection);
        return new Response(200, "Word Deleted");
    }

    public async Task<Response> GetWords(string collectionId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        if (collection == null) return new Response(404, "Collection Not Found");
        ;
        var words = collection.Words;
        return words.Count == 0
            ? new Response(404, "Collection don't have words")
            : new Response(200, "Words Retrieved", words);
    }

    public async Task<Response> UpdateWord(string collectionId, string wordId, string? text, string? translate,
        string? description)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        var word = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));

        if (word == null) return new Response(404, "Collection Not Found");
        ;
        if (text != null) word.Text = text;
        if (translate != null) word.Translate = translate;

        await context.WordsCollections.ReplaceOneAsync(w => w.Id == new ObjectId(collectionId), collection);

        return new Response(200, "Word Updated", word);
    }

    public async Task<Response> GetWordById(string collectionId, string wordId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        if (collection == null) return new Response(404, "Collection Not Found");
        var word = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));
        return word == null ? new Response(404, "Word Not Found") : new Response(200, "Word Retrieved", word);
    }

    #endregion

    //Examples-----------------------

    #region Examples

    public async Task<Response> AddExample(CreateExampleFrom example, string collectionId, string wordId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        if (collection == null) return new Response(404, "Collection Not Found");
        var word = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));
        if (word == null) return new Response(404, "Word Not Found");
        var newExample = new Example
        {
            Id = ObjectId.GenerateNewId(),
            Text = example.Text,
            Translate = example.Translate
        };
        word.Examples.Add(newExample);
        var result =
            await context.WordsCollections.ReplaceOneAsync(w => w.Id == new ObjectId(collectionId), collection);
        if (result.ModifiedCount == 0) return new Response(404, "Collection Not Found");
        ;
        return new Response(200, "Example Created", newExample);
    }

    public async Task<Response> AddExamples(List<CreateExampleFrom> examples, string collectionId, string wordId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        if (collection == null) return new Response(404, "Collection Not Found");
        var word = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));
        if (word == null) return new Response(404, "Word Not Found");
        foreach (var example in examples)
        {
            var newExample = new Example
            {
                Id = ObjectId.GenerateNewId(),
                Text = example.Text,
                Translate = example.Translate
            };
            word.Examples.Add(newExample);
        }

        var result =
            await context.WordsCollections.ReplaceOneAsync(w => w.Id == new ObjectId(collectionId), collection);
        if (result.ModifiedCount == 0) return new Response(404, "Collection Not Found");
        ;
        return new Response(200, "Examples Created");
    }

    public async Task<Response> DeleteExample(string collectionId, string wordId, string exampleId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        if (collection == null) return new Response(404, "Collection Not Found");
        var word = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));
        if (word == null) return new Response(404, "Word Not Found");
        var example = word.Examples.FirstOrDefault(w => w.Id == new ObjectId(exampleId));
        if (example == null) return new Response(404, "Example Not Found");
        word.Examples.Remove(example);
        await context.WordsCollections.ReplaceOneAsync(w => w.Id == new ObjectId(collectionId), collection);
        return new Response(200, "Example Deleted");
    }

    public async Task<Response> GetExamples(string collectionId, string wordId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        if (collection == null) return new Response(404, "Collection Not Found");
        var word = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));
        if (word == null) return new Response(404, "Word Not Found");
        var examples = word.Examples;
        return new Response(200, "Examples Retrieved", examples);
    }

    public async Task<Response> UpdateExample(string collectionId, string wordId, string exampleId, string? text,
        string? translate)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        if (collection == null) return new Response(404, "Collection Not Found");
        var word = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));
        if (word == null) return new Response(404, "Word Not Found");
        var example = word.Examples.FirstOrDefault(e => e.Id == new ObjectId(exampleId));
        if (example == null) return new Response(404, "Example Not Found");
        if (text != null) example.Text = text;
        if (translate != null) example.Translate = translate;
        var result = context.WordsCollections.ReplaceOneAsync(w => w.Id == new ObjectId(collectionId), collection);
        return new Response(200, "Example Updated", example);
    }

    public async Task<Response> UpdateExamples(List<CreateExampleFrom> examples, string collectionId, string wordId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        if (collection == null) return new Response(404, "Collection Not Found");
        var word = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));
        if (word == null) return new Response(404, "Word Not Found");
        word.Examples.Clear();
        foreach (var example in examples)
            word.Examples.Add(new Example
            {
                Id = ObjectId.GenerateNewId(),
                Text = example.Text,
                Translate = example.Translate
            });
        await context.WordsCollections.ReplaceOneAsync(w => w.Id == new ObjectId(collectionId), collection);
        return new Response(200, "Examples Updated");
    }

    public async Task<Response> GetExampleById(string collectionId, string wordId, string exampleId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId))
            .FirstOrDefaultAsync();
        if (collection == null) return new Response(404, "Collection Not Found");
        var word = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));
        if (word == null) return new Response(404, "Word Not Found");
        var example = word.Examples.FirstOrDefault(w => w.Id == new ObjectId(exampleId));
        if (example == null) return new Response(404, "Example Not Found");
        return new Response(200, "Example Retrieved", example);
    }

    #endregion
}