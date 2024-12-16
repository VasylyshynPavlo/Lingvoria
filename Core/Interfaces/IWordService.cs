using Core.Models;
using Core.Models.LibraryModels.Create;

namespace Core.Interfaces;

public interface IWordService
{
    #region OtherLogic

    Task<bool> IsMyCollection(string collectionId, string userId);

    #endregion

    //Collections-----------------------------------

    #region Collections

    Task<Response> CreateCollection(CreateWordsCollectionForm collectionModel, string userId);
    Task<Response> DeleteCollection(string collectionId, string userId);
    Task<Response> GetCollections(string userId);
    Task<Response> GetCollectionById(string collectionId, string userId);
    Task<Response> UpdateCollection(string collectionId, string? language, string? title, string userId);

    #endregion

    //Words-----------------------------------

    #region Words

    Task<Response> AddWord(CreateWordForm word, string collectionId);
    Task<Response> AddWords(List<CreateWordForm> words, string collectionId);
    Task<Response> DeleteWord(string collectionId, string wordId);
    Task<Response> GetWords(string collectionId);
    Task<Response> UpdateWord(string collectionId, string wordId, string? language, string? title, string? description);
    Task<Response> GetWordById(string collectionId, string wordId);

    #endregion

    //Examples

    #region Examples

    Task<Response> AddExample(CreateExampleFrom example, string collectionId, string wordId);
    Task<Response> AddExamples(List<CreateExampleFrom> examples, string collectionId, string wordId);
    Task<Response> DeleteExample(string collectionId, string wordId, string exampleId);
    Task<Response> GetExamples(string collectionId, string wordId);
    Task<Response> UpdateExample(string collectionId, string wordId, string exampleId, string? text, string? translate);
    Task<Response> UpdateExamples(List<CreateExampleFrom> examples, string collectionId, string wordId);
    Task<Response> GetExampleById(string collectionId, string wordId, string exampleId);

    #endregion
}