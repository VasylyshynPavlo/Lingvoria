using System.Security.Claims;
using Core.Interfaces;
using Core.Models.LibraryModels;
using Core.Models.LibraryModels.Create;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LingvoriaAPI.Controllers
{
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
                    response.Code,
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
        
        #endregion
    }
}
