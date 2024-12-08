using Core.Models;
using Core.Models.LibraryModels.Create;
using MongoDB.Bson;

namespace Core.Interfaces;

public interface IWordService
{
    //Collections-----------------------------------
    #region Collections
    
    Task<Response> CreateCollection(CreateWordsCollectionForm collectionModel, string userId);
    Task<Response> DeleteCollection(string collectionId, string userId);
    Task<Response> GetCollections(string userId);
    Task<Response> UpdateCollection(string collectionId, string? language, string? title, string userId);
    Task<Response> GetCollectionById(string collectionId);
    
    #endregion
    
    //Words-----------------------------------
    #region Words
    
    Task<Response> AddWord(CreateWordForm word, string collectionId);
    Task<Response> DeleteWord(string collectionId, string wordId);
    Task<Response> GetWords(string collectionId);
    Task<Response> UpdateWord(string collectionId, string wordId, string? language, string? title, string? description);
    Task<Response> GetWordById(string collectionId, string wordId);
    
    #endregion
    
    //Examples
    #region Examples
    
    Task<Response> AddExample(CreateExampleFrom example);
    Task<Response> DeleteExample(string collectionId, string wordId, string exampleId);
    Task<Response> GetExamples(string collectionId, string wordId);
    Task<Response> UpdateExample(string collectionId, string wordId, string exampleId, string? text, string? translate);
    Task<Response> GetExampleById(string collectionId, string wordId, string exampleId);
    
    #endregion
    
    
    #region Logic
    
    Task<bool> IsMyCollection(string collectionId, string userId);
    
    #endregion
}