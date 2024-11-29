using AutoMapper;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LingvoriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordsController : ControllerBase
    {
        private readonly LingvoriaDbContext _context;
        private readonly IMapper _mapper;
        private readonly WordService _wordService;
        public WordsController(IMapper mapper)
        {
            this._context = new LingvoriaDbContext("mongodb://localhost:27017", "LingvoriaDb");
            this._mapper = mapper;
            this._wordService = new WordService(_context, mapper);
        }
        //--------------------POST--------------------
        #region POST
        
        [HttpPost()]
        public async Task<IActionResult> CreateWordsCollection([FromForm] CreateWordsCollectionModel wordsCollectionModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _wordService.CreateCollection(wordsCollectionModel);
            if (result.Code == "200") return Ok();
            return BadRequest("Something Went Wrong");
        }

        [HttpPost("words")]
        public async Task<IActionResult> AddWord([FromForm] CreateWordModel wordModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _wordService.AddWord(wordModel);
            if (result.Code == "200") return Ok();
            else if (result.Code == "404") return NotFound(result.Message);
            else return BadRequest("Something Went Wrong");
        }

        [HttpPost("examples")]
        public async Task<IActionResult> AddExample([FromForm] CreateExampleModel wordExampleModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _wordService.AddExample(wordExampleModel);
            if (result.Code == "200") return Ok(result.Data);
            else if (result.Code == "404") return NotFound(result.Message);
            else return BadRequest("Something Went Wrong");
        }
        
        #endregion
        
        //--------------------GET--------------------
        #region GET
        
        
        [HttpGet()]
        public async Task<IActionResult> GetAllWordsCollections()
        {
            var result = await _wordService.GetCollections();
            if (result.Code == "200")
            {
                if (result.Data is List<WordsCollectionModel> collections)
                {
                    var responseData = collections.Select(c => new
                    {
                        Id = c.Id.ToString(),
                        Title = c.Title,
                        UserId = c.UserId,
                        Language = c.Language,
                        Words = c.Words.Select(w => new
                        {
                            Id = w.Id.ToString(),
                            Word = w.Word,
                            Description = w.Description,
                            Translate = w.Translate,
                            Examples = w.Examples.Select(e => new
                            {
                                Id = e.Id.ToString(),
                                Text = e.Text,
                                Tranlate = e.Translate,
                            })
                        }).ToList()
                    }).ToList();

                    return Ok(responseData);
                }
                else
                {
                    return BadRequest("Data is not in the expected format.");
                }
            }
            else if (result.Code == "404") return NotFound(result.Message);
            else return BadRequest("Something Went Wrong");
        }
        
        
        
        [HttpGet("{colectionId}/words")]
        public async Task<IActionResult> GetWordsByCollection(string colectionId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _wordService.GetWords(colectionId);
            if (result.Code == "200")
            {
                if (result.Data is List<WordModel> words)
                {
                    var responseData = words.Select(w => new
                    {
                        Id = w.Id.ToString(),
                        Word = w.Word,
                        Description = w.Description,
                        Translate = w.Translate,
                        Examples = w.Examples.Select(e => new
                        {
                            Id = e.Id.ToString(),
                            Text = e.Text,
                            Tranlate = e.Translate,
                        }).ToList()
                    }).ToList();
                    return Ok(responseData);
                }
                else return BadRequest("Data is not in the expected format.");
            }
            else if (result.Code == "404") return NotFound(result.Message);
            else return BadRequest("Something Went Wrong");
        }

        [HttpGet("{colectionId}/words/{wordId}")]
        public async Task<IActionResult> GetWordById(string colectionId, string wordId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _wordService.GetWordById(colectionId, wordId);
            if (result.Code == "200")
            {
                if (result.Data is WordModel word)
                {
                    var responseData = new
                    {
                        Id = word.Id.ToString(),
                        word.Word,
                        word.Description,
                        word.Translate,
                        Examples = word.Examples.Select(e => new
                        {
                            Id = e.Id.ToString(),
                            Text = e.Text,
                            Tranlate = e.Translate,
                        }).ToList()
                    };
                    return Ok(responseData);
                }
                else return BadRequest("Data is not in the expected format.");
            }
            else if (result.Code == "404") return NotFound(result.Message);
            else return BadRequest("Something Went Wrong");
        }
        #endregion

    }
}
