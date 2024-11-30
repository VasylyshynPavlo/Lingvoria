using AutoMapper;
using Core.Interfaces;
using Core.Models;
using Core.Models.Create;
using Core.Models.Update;
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
        
        [HttpPost("collection")]
        public async Task<IActionResult> CreateWordsCollection([FromBody] CreateWordsCollectionModel wordsCollectionModel)
        {
            // // //if (!ModelState.IsValid) return BadRequest(ModelState);
            // // var newCollection = new WordsCollectionModel
            // // {
            // //     Id = ObjectId.GenerateNewId(),
            // //     UserId = wordsCollectionModel.UserId,
            // //     Language = wordsCollectionModel.Language,
            // //     Title = wordsCollectionModel.Title,
            // //     Words = []
            // // };
            // //await _context.WordsCollections.InsertOneAsync(newCollection);
            // return Ok();
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

        [HttpGet("{collectionId}")]
        public async Task<IActionResult> GetWordsCollectionById(string collectionId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _wordService.GetCollectionById(collectionId);
            if (result.Code == "200")
            {
                if (result.Data is WordsCollectionModel collection)
                {
                    var responseData = new
                    {
                        Id = collection.Id.ToString(),
                        Title = collection.Title,
                        UserId = collection.UserId,
                        Language = collection.Language,
                        Words = collection.Words.Select(w => new
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
                        }).ToList()
                    };
                    return Ok(responseData);
                }
                else return BadRequest("Data is not in the expected format.");
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

        [HttpGet("{colectionId}/words/{wordId}/examples")]
        public async Task<IActionResult> GetExamplesByCollectionAndWord(string colectionId, string wordId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _wordService.GetExamples(colectionId, wordId);
            if (result.Code == "200")
            {
                if (result.Data is List<ExampleModel> examples)
                {
                    var responseData = examples.Select(e => new
                    {
                        Id = e.Id.ToString(),
                        Text = e.Text,
                        Tranlate = e.Translate,
                    }).ToList();
                    return Ok(responseData);
                }
                else return BadRequest("Data is not in the expected format.");
            }
            else if (result.Code == "404") return NotFound(result.Message);
            else return BadRequest("Something Went Wrong");
        }

        [HttpGet("{colectionId}/words/{wordId}/examples/{exampleId}")]
        public async Task<IActionResult> GetExampleById(string colectionId, string wordId, string exampleId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _wordService.GetExampleById(colectionId, wordId, exampleId);
            if (result.Code == "200")
            {
                if (result.Data is ExampleModel example)
                {
                    var responseData = new
                    {
                        Id = example.Id.ToString(),
                        Text = example.Text,
                        Tranlate = example.Translate,
                    };
                    return Ok(responseData);
                }
                else return BadRequest("Data is not in the expected format.");
            }
            else if (result.Code == "404") return NotFound(result.Message);
            else return BadRequest("Something Went Wrong");
        }
        
        #endregion

        #region PUT

        // [HttpPut("{colectionId}/words/{wordId}")]
        // public async Task<IActionResult> UpdateWordsCollection([FromForm] UpdateWordCollectionModel model)
        // {
        //     if (!ModelState.IsValid) return BadRequest(ModelState);
        //     var result = await _wordService.UpdateCollection(model);
        //     if (result.Code == "200")
        //     {
        //         if (result.Data is List<WordsCollectionModel> collections)
        //         {
        //             var responseData =
        //         }
        //     }
        // }

        #endregion
    }
}
