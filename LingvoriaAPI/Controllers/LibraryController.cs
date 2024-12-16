using System.Security.Claims;
using Core.Interfaces;
using Core.Models.LibraryModels;
using Core.Models.LibraryModels.Create;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LingvoriaAPI.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class LibraryController(IWordService wordService) : ControllerBase
{
    #region WordsCollections

    //--------Create WordsCollection And Add To Database--------
    [HttpPost("collections")]
    public async Task<IActionResult> CreateCollection([FromForm] CreateWordsCollectionForm form)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var response = await wordService.CreateCollection(form, userId);
        return Ok(response);
    }

    //--------Delete WordsCollection And Refresh Database--------
    [HttpDelete("collections")]
    public async Task<IActionResult> DeleteCollection(string collectionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var response = await wordService.DeleteCollection(collectionId, userId);
        return response.Code switch
        {
            200 => Ok(response),
            403 => Forbid(new AuthenticationProperties { Items = { { "Reason", response.Message } } }),
            404 => NotFound(response),
            _ => BadRequest("Something went wrong")
        };
    }

    //--------Get WordsCollection And Refresh Database--------
    [HttpGet("collections")]
    public async Task<IActionResult> GetCollections()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var response = await wordService.GetCollections(userId);
        return response switch
        {
            { Code: 200, Data: List<WordsCollection> collections } => Ok(new
            {
                response.Code,
                Data = collections.Select(c => c.ToDto()).ToList()
            }),
            { Code: 404 } => NotFound(response),
            _ => BadRequest("Something went wrong.")
        };
    }

    //--------Get WordsCollection And Refresh Database--------
    [HttpPut("collections")]
    public async Task<IActionResult> UpdateCollection([FromForm] CreateWordsCollectionForm form)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var response = await wordService.CreateCollection(form, userId);
        return response switch
        {
            { Code: 200, Data: WordsCollection collection } => Ok(new
            {
                Data = collection.ToDto()
            }),
            { Code: 403 } => Forbid(new AuthenticationProperties { Items = { { "Reason", response.Message } } }),
            { Code: 404 } => NotFound(new
            {
                response.Code,
                response.Message
            }),
            _ => BadRequest("Something went wrong.")
        };
    }

    [HttpGet("collections-by-id")]
    public async Task<IActionResult> GetCollectionById(string collectionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var response = await wordService.GetCollectionById(collectionId, userId);
        return response switch
        {
            { Code: 200, Data: WordsCollection collection } => Ok(collection.ToDto()),
            { Code: 403 } => Forbid(new AuthenticationProperties { Items = { { "Reason", response.Message } } }),
            { Code: 404 } => NotFound(response),
            _ => BadRequest("Something went wrong.")
        };
    }

    #endregion

    #region Words

    [HttpPost("words")]
    public async Task<IActionResult> AddWord([FromForm] CreateWordForm form, string collectionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (!await wordService.IsMyCollection(collectionId, userId))
            return Forbid(new AuthenticationProperties
                { Items = { { "Reason", "You are not the owner of this collection" } } });
        var response = await wordService.AddWord(form, collectionId);
        return response.Code switch
        {
            200 => Ok(response),
            404 => NotFound(response),
            _ => BadRequest("Something went wrong.")
        };
    }

    [HttpPost("words/many")]
    public async Task<IActionResult> AddWords(List<CreateWordForm> words, string collectionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (!await wordService.IsMyCollection(collectionId, userId))
            return Forbid(new AuthenticationProperties
                { Items = { { "Reason", "You are not the owner of this collection" } } });
        var response = await wordService.AddWords(words, collectionId);
        return response.Code switch
        {
            200 => Ok(response),
            404 => NotFound(response),
            _ => BadRequest("Something went wrong.")
        };
    }

    [HttpDelete("words")]
    public async Task<IActionResult> DeleteWord(string collectionId, string wordId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (!await wordService.IsMyCollection(collectionId, userId))
            return Forbid(new AuthenticationProperties
                { Items = { { "Reason", "You are not the owner of this collection" } } });
        var response = await wordService.DeleteWord(collectionId, wordId);
        return response.Code switch
        {
            200 => Ok(response),
            404 => NotFound(response),
            _ => BadRequest("Something went wrong.")
        };
    }

    [HttpGet("words")]
    public async Task<IActionResult> GetWords(string collectionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (!await wordService.IsMyCollection(collectionId, userId))
            return Forbid(new AuthenticationProperties
                { Items = { { "Reason", "You are not the owner of this collection" } } });
        var response = await wordService.GetWords(collectionId);
        return response switch
        {
            { Code: 200, Data: List<Word> words } => Ok(new
            {
                Data = words.Select(c => c.ToDto()).ToList()
            }),
            { Code: 404 } => NotFound(response),
            _ => BadRequest("Something went wrong.")
        };
    }

    [HttpPut("words")]
    public async Task<IActionResult> UpdateWord(string collectionId, string wordId, string? text, string? translate,
        string? description)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (!await wordService.IsMyCollection(collectionId, userId))
            return Forbid(new AuthenticationProperties
                { Items = { { "Reason", "You are not the owner of this collection" } } });
        var response = await wordService.UpdateWord(collectionId, wordId, text, translate, description);
        return response switch
        {
            { Code: 200, Data: Word word } => Ok(new
            {
                Data = word.ToDto()
            }),
            { Code: 404 } => NotFound(response),
            _ => BadRequest("Something went wrong.")
        };
    }

    [HttpGet("words-by-id")]
    public async Task<IActionResult> GetWordsById(string collectionId, string wordId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (!await wordService.IsMyCollection(collectionId, userId))
            return Forbid(new AuthenticationProperties
                { Items = { { "Reason", "You are not the owner of this collection" } } });
        var response = await wordService.GetWordById(collectionId, wordId);
        return response switch
        {
            { Code: 200, Data: Word word } => Ok(word.ToDto()),
            { Code: 404 } => NotFound(response),
            _ => BadRequest("Something went wrong.")
        };
    }

    #endregion

    #region Examples

    [HttpPost("examples")]
    public async Task<IActionResult> AddExample([FromForm] CreateExampleFrom example, string collectionId,
        string wordId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (!await wordService.IsMyCollection(collectionId, userId))
            return Forbid(new AuthenticationProperties
                { Items = { { "Reason", "You are not the owner of this collection" } } });
        var response = await wordService.AddExample(example, collectionId, wordId);
        return response.Code switch
        {
            200 => Ok(response),
            404 => NotFound(response),
            _ => BadRequest("Something went wrong.")
        };
    }

    [HttpPost("examples/many")]
    public async Task<IActionResult> AddExamples(List<CreateExampleFrom> examples, string collectionId,
        string wordId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (!await wordService.IsMyCollection(collectionId, userId))
            return Forbid(new AuthenticationProperties
                { Items = { { "Reason", "You are not the owner of this collection" } } });
        var response = await wordService.AddExamples(examples, collectionId, wordId);
        return response.Code switch
        {
            200 => Ok(response),
            404 => NotFound(response),
            _ => BadRequest("Something went wrong.")
        };
    }

    [HttpDelete("examples")]
    public async Task<IActionResult> DeleteExample(string collectionId, string wordId, string exampleId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (!await wordService.IsMyCollection(collectionId, userId))
            return Forbid(new AuthenticationProperties
                { Items = { { "Reason", "You are not the owner of this collection" } } });
        var response = await wordService.DeleteExample(collectionId, wordId, exampleId);
        return response.Code switch
        {
            200 => Ok(response),
            404 => NotFound(response),
            _ => BadRequest("Something went wrong.")
        };
    }

    [HttpGet("examples")]
    public async Task<IActionResult> GetExamples(string collectionId, string wordId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (!await wordService.IsMyCollection(collectionId, userId))
            return Forbid(new AuthenticationProperties
                { Items = { { "Reason", "You are not the owner of this collection" } } });
        var response = await wordService.GetExamples(collectionId, wordId);
        return response switch
        {
            { Code: 200, Data: List<Example> examples } => Ok(new
            {
                Data = examples.Select(c => c.ToDto()).ToList()
            }),
            { Code: 404 } => NotFound(response),
            _ => BadRequest("Something went wrong.")
        };
    }

    [HttpPut("examples")]
    public async Task<IActionResult> UpdateExample(string collectionId, string wordId, string exampleId,
        string? text, string? translate)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (!await wordService.IsMyCollection(collectionId, userId))
            return Forbid(new AuthenticationProperties
                { Items = { { "Reason", "You are not the owner of this collection" } } });
        var response = await wordService.UpdateExample(collectionId, wordId, exampleId, text, translate);
        return response.Code switch
        {
            200 => Ok(response),
            404 => NotFound(response),
            _ => BadRequest("Something went wrong.")
        };
    }

    [HttpPut("examples/many")]
    public async Task<IActionResult> UpdateExamples(List<CreateExampleFrom> examples, string collectionId,
        string wordId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (!await wordService.IsMyCollection(collectionId, userId))
            return Forbid(new AuthenticationProperties
                { Items = { { "Reason", "You are not the owner of this collection" } } });
        var response = await wordService.UpdateExamples(examples, collectionId, wordId);
        return response.Code switch
        {
            200 => Ok(response),
            404 => NotFound(response),
            _ => BadRequest("Something went wrong.")
        };
    }

    [HttpGet("examples-by-id")]
    public async Task<IActionResult> GetExampleById(string collectionId, string wordId, string exampleId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (!await wordService.IsMyCollection(collectionId, userId))
            return Forbid(new AuthenticationProperties
                { Items = { { "Reason", "You are not the owner of this collection" } } });
        var response = await wordService.GetExampleById(collectionId, wordId, exampleId);
        return response switch
        {
            { Code: 200, Data: Example example } => Ok(example.ToDto()),
            { Code: 404 } => NotFound(response),
            _ => BadRequest("Something went wrong.")
        };
    }

    #endregion
}