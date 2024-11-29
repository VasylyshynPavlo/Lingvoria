using Core.Models;
using MongoDB.Bson;

namespace Core.Interfaces;

public interface IWordService
{
    //Collections-----------------------------------
    #region Collections
    
    Task<Result> CreateCollection(CreateWordsCollectionModel collectionModel);
    Task<Result> DeleteCollection(string collectionId);
    Task<Result> GetCollections();
    Task<Result> UpdateCollection(string collectionId, string language, string title);
    Task<Result> GetCollectionById(string collectionId);
    
    #endregion
    
    //Words-----------------------------------
    #region Words
    
    Task<Result> AddWord(CreateWordModel word);
    Task<Result> DeleteWord(string collectionId, string wordId);
    Task<Result> GetWords(string collectionId);
    Task<Result> UpdateWord(string collectionId, string wordId, string language, string title);
    Task<Result> GetWordById(string collectionId, string wordId);
    
    #endregion
    
    //Examples
    #region Examples
    
    Task<Result> AddExample(CreateExampleModel example);
    Task<Result> DeleteExample(string exampleId);
    Task<Result> GetExamples(string collectionId, string wordId);
    Task<Result> UpdateExample(string collectionId, string wordId, string exampleId, string? text, string? translate);
    Task<Result> GetExampleById(string collectionId, string wordId, string exampleId);
    
    #endregion
}