using AutoMapper;
using Core.Interfaces;
using Core.Models;
using Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Core.Services;

public class WordService(LingvoriaDbContext context, IMapper mapper) : IWordService
{
    //Collections-----------------------------------
    #region Collections
    public async Task<Result> CreateCollection(CreateWordsCollectionModel collectionModel)
    {
        var newCollection = new WordsCollectionModel
        {
            Id = ObjectId.GenerateNewId(),
            UserId = collectionModel.UserId,
            Language = collectionModel.Language,
            Title = collectionModel.Title,
            Words = new List<WordModel>()
        };
        await context.WordsCollections.InsertOneAsync(newCollection);
        
        return new Result("200", "Collection Created");
    }

    public async Task<Result> DeleteCollection(string collectionId)
    {
        var result = await context.WordsCollections.DeleteOneAsync(w => w.Id == new ObjectId(collectionId));
        if (result.DeletedCount == 0)
                 return new Result("404", "Collection Not Found");
        return new Result("200", "Collection Deleted");
    }

    public async Task<Result> GetCollections()
    {
        var collections = await context.WordsCollections.Find(_ => true).ToListAsync();
        if (collections == null || collections.Count == 0) return new Result("404", "Collections Not Found");
        return new Result("200", "Collections Retrieved", collections);
    }

    public async Task<Result> UpdateCollection(string collectionId, string? language, string? title)
    {
        var collection = mapper.Map<WordsCollectionModel>(await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId)).FirstOrDefaultAsync());
        
        if (collection == null) return new Result("404", "Collection Not Found");;
        if (language != null) collection.Language = language;
        if (title != null) collection.Title = title;

        await context.WordsCollections.ReplaceOneAsync(w => w.Id == new ObjectId(collectionId), collection);
        
        return new Result("200", "Collection Updated");
    }

    public async Task<Result> GetCollectionById(string collectionId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId)).FirstOrDefaultAsync();
        if (collection == null) return new Result("404", "Collection Not Found");
        return new Result("200", "Collection Retrieved", collection);
    }
    
    #endregion

    //Words-----------------------------------
    #region Words
    public async Task<Result> AddWord(CreateWordModel word)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(word.CollectionId)).FirstOrDefaultAsync();
        var newWord = new WordModel
        {
            Id = ObjectId.GenerateNewId(),
            Word = word.Word,
            Translate = word.Translate,
            Description = word.Description,
            Examples = new List<ExampleModel>()
        };
        collection.Words.Add(newWord);
        var result = await context.WordsCollections.ReplaceOneAsync(w => w.Id == new ObjectId(word.CollectionId), collection);
        if (result.ModifiedCount == 0) return new Result("404", "Collection Not Found");
        return new Result("200", "Word Created");
    }
    
    public async Task<Result> DeleteWord(string collectionId, string wordId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId)).FirstOrDefaultAsync();
        var wordToRemove = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));
        if (wordToRemove == null) return new Result("404", "Word Not Found");
        collection.Words.Remove(wordToRemove);
        await context.WordsCollections.ReplaceOneAsync(w => w.Id == new ObjectId(collectionId), collection);
        return new Result("200", "Word Deleted");
    }
    public async Task<Result> GetWords(string collectionId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId)).FirstOrDefaultAsync();
        if (collection == null) return new Result("404", "Collection Not Found");;
        var words = collection.Words;
        if (words.Count == 0) return new Result("404", "Words Not Found");
        return new Result("200", "Words Retrieved", words);
    }
    public async Task<Result> UpdateWord(string collectionId, string wordId, string? text, string? translate)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId)).FirstOrDefaultAsync();
        var word = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));
        
        if (word == null) return new Result("404", "Collection Not Found");;
        if (text != null) word.Word = text;
        if (translate != null) word.Translate = translate;

        await context.WordsCollections.ReplaceOneAsync(w => w.Id == new ObjectId(collectionId), collection);
        
        return new Result("200", "Word Updated");
    }

    public async Task<Result> GetWordById(string collectionId, string wordId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId)).FirstOrDefaultAsync();
        if (collection == null) return new Result("404", "Collection Not Found");
        var word = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));
        if (word == null) return new Result("404", "Word Not Found");
        
        return new Result("200", "Word Retrieved", word);
    }

    #endregion

    //Examples-----------------------
    #region Examples
    
    public async Task<Result> AddExample(CreateExampleModel example)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(example.collectionId)).FirstOrDefaultAsync();
        if (collection == null) return new Result("404", "Collection Not Found");;
        var word = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(example.wordId));
        if (word == null) return new Result("404", "Word Not Found");;
        var newExample = new ExampleModel
        {
            Id = ObjectId.GenerateNewId(),
            Text = example.Text,
            Translate = example.Translate,
        };
        word.Examples.Add(newExample);
        var result = await context.WordsCollections.ReplaceOneAsync(w => w.Id == new ObjectId(example.collectionId), collection);
        if (result.ModifiedCount == 0) return new Result("404", "Collection Not Found");;
        return new Result("200", "Example Created", newExample);
    }
    public async Task<Result> DeleteExample(string exampleId)
    {
        var result = await context.Examples.DeleteOneAsync(w => w.Id == new ObjectId(exampleId));
        if (result.DeletedCount == 0) return new Result("404", "Example Not Found");;
        return new Result("200", "Example Deleted");
    }
    public async Task<Result> GetExamples(string collectionId, string wordId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId)).FirstOrDefaultAsync();
        if (collection == null) return new Result("404", "Collection Not Found");
        var word = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));
        if (word == null) return new Result("404", "Word Not Found");
        var examples = word.Examples;
        return new Result("200", "Examples Retrieved", examples);
    }
    public async Task<Result> UpdateExample(string collectionId, string wordId, string exampleId, string text, string translate)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId)).FirstOrDefaultAsync();
        if (collection == null) return new Result("404", "Collection Not Found");
        var word = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));
        if (word == null) return new Result("404", "Word Not Found");
        var example = word.Examples.FirstOrDefault(w => w.Id == new ObjectId(exampleId));
        if (example == null) return new Result("404", "Example Not Found");
        if (text != null) example.Text = text;
        if (translate != null) example.Translate = translate;
        var result = context.Examples.ReplaceOneAsync(w => w.Id == new ObjectId(exampleId), example);
        if (result.Result.ModifiedCount == 0) return new Result("404", "Example Not Found");
        return new Result("200", "Example Updated");
    }
    public async Task<Result> GetExampleById(string collectionId, string wordId, string exampleId)
    {
        var collection = await context.WordsCollections.Find(w => w.Id == new ObjectId(collectionId)).FirstOrDefaultAsync();
        if (collection == null) return new Result("404", "Collection Not Found");
        var word = collection.Words.FirstOrDefault(w => w.Id == new ObjectId(wordId));
        if (word == null) return new Result("404", "Word Not Found");
        var example = word.Examples.FirstOrDefault(w => w.Id == new ObjectId(exampleId));
        if (example == null) return new Result("404", "Example Not Found");
        return new Result("200", "Example Retrieved", example);
    }
    
    #endregion
}