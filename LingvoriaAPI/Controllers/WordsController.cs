using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace LingvoriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordsController : ControllerBase
    {
        private readonly LingvoriaDbContext _context;
        
        public WordsController()
        {
            // Підключення до MongoDB
            _context = new LingvoriaDbContext("mongodb://localhost:27017", "LingvoriaDb");
        }

        [HttpPost()]
        public async Task<IActionResult> CreateWordsCollection(WordsCollection collection)
        {
            await _context.WordsCollections.InsertOneAsync(collection);
            return Ok(collection);
        }
        
        [HttpPost("{collectionId}/word")]
        public async Task<IActionResult> AddWordToCollection(int collectionId, Word word)
        {
            var collection = await _context.WordsCollections.Find(w => w.Id == collectionId).FirstOrDefaultAsync();
            if (collection == null)
                return NotFound("Collection not found");

            collection.Words.Add(word);
            await _context.WordsCollections.ReplaceOneAsync(w => w.Id == collectionId, collection);
            return Ok(word);
        }
        
        [HttpPost("{collectionId}/word/{wordId}/example")]
        public async Task<IActionResult> AddExampleToWord(int collectionId, string wordId, Example example)
        {
            var collection = await _context.WordsCollections.Find(w => w.Id == collectionId).FirstOrDefaultAsync();
            if (collection == null)
                return NotFound("Collection not found");

            var word = collection.Words.FirstOrDefault(w => w.WordText == wordId);
            if (word == null)
                return NotFound("Word not found");

            word.Examples.Add(example);
            await _context.WordsCollections.ReplaceOneAsync(w => w.Id == collectionId, collection);
            return Ok(example);
        }
        
        [HttpGet()]
        public async Task<IActionResult> GetAllWordsCollections()
        {
            var collections = await _context.WordsCollections.Find(_ => true).ToListAsync();
            return Ok(collections);
        }
        
        [HttpGet("{collectionId}")]
        public async Task<IActionResult> GetWordsCollectionById(int collectionId)
        {
            var collection = await _context.WordsCollections.Find(w => w.Id == collectionId).FirstOrDefaultAsync();
            if (collection == null)
                return NotFound("Collection not found");

            return Ok(collection);
        }
        
        [HttpGet("{collectionId}/words")]
        public async Task<IActionResult> GetWordsInCollection(int collectionId)
        {
            var collection = await _context.WordsCollections.Find(w => w.Id == collectionId).FirstOrDefaultAsync();
            if (collection == null)
                return NotFound("Collection not found");

            return Ok(collection.Words);
        }
        
        [HttpGet("{collectionId}/word/{wordText}")]
        public async Task<IActionResult> GetWordInCollection(int collectionId, string wordText)
        {
            var collection = await _context.WordsCollections.Find(w => w.Id == collectionId).FirstOrDefaultAsync();
            if (collection == null)
                return NotFound("Collection not found");

            var word = collection.Words.FirstOrDefault(w => w.WordText == wordText);
            if (word == null)
                return NotFound("Word not found");

            return Ok(word);
        }
        
        [HttpGet("{collectionId}/word/{wordText}/examples")]
        public async Task<IActionResult> GetExamplesForWord(int collectionId, string wordText)
        {
            var collection = await _context.WordsCollections.Find(w => w.Id == collectionId).FirstOrDefaultAsync();
            if (collection == null)
                return NotFound("Collection not found");

            var word = collection.Words.FirstOrDefault(w => w.WordText == wordText);
            if (word == null)
                return NotFound("Word not found");

            return Ok(word.Examples);
        }
        
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetWordsCollectionsByUserId(string userId)
        {
            var collections = await _context.WordsCollections
                .Find(w => w.UserId == userId)
                .ToListAsync();

            if (collections.Count == 0)
                return NotFound("No collections found for this user");

            return Ok(collections);
        }

        [HttpDelete("{collectionId}/word/{wordId}/example/{exampleId}")]
        public async Task<IActionResult> DeleteExampleFromWord(int collectionId, string wordId, int exampleId)
        {
            var collection = await _context.WordsCollections.Find(w => w.Id == collectionId).FirstOrDefaultAsync();
            if (collection == null)
                return NotFound("Collection not found");

            var word = collection.Words.FirstOrDefault(w => w.WordText == wordId);
            if (word == null)
                return NotFound("Word not found");

            var example = word.Examples.FirstOrDefault(e => e.Id == exampleId);
            if (example == null)
                return NotFound("Example not found");

            word.Examples.Remove(example);
            await _context.WordsCollections.ReplaceOneAsync(w => w.Id == collectionId, collection);

            return Ok("Example deleted successfully");
        }

        [HttpDelete("{collectionId}/word/{wordId}")]
        public async Task<IActionResult> DeleteWordFromCollection(int collectionId, string wordId)
        {
            var collection = await _context.WordsCollections.Find(w => w.Id == collectionId).FirstOrDefaultAsync();
            if (collection == null)
                return NotFound("Collection not found");

            var word = collection.Words.FirstOrDefault(w => w.WordText == wordId);
            if (word == null)
                return NotFound("Word not found");

            collection.Words.Remove(word);
            await _context.WordsCollections.ReplaceOneAsync(w => w.Id == collectionId, collection);

            return Ok("Word deleted successfully");
        }

        [HttpDelete("{collectionId}")]
        public async Task<IActionResult> DeleteWordsCollection(int collectionId)
        {
            var result = await _context.WordsCollections.DeleteOneAsync(w => w.Id == collectionId);
            if (result.DeletedCount == 0)
                return NotFound("Collection not found");

            return Ok("Collection deleted successfully");
        }

        [HttpPut("{collectionId}")]
        public async Task<IActionResult> UpdateWordsCollection(int collectionId, WordsCollection updatedCollection)
        {
            if (updatedCollection == null)
            {
                return BadRequest("Updated collection data is null");
            }

            var existingCollection = await _context.WordsCollections.Find(w => w.Id == collectionId).FirstOrDefaultAsync();
            if (existingCollection == null)
                return NotFound("Collection not found");
            
            if (!string.IsNullOrEmpty(updatedCollection.Language) && updatedCollection.Language != existingCollection.Language)
                existingCollection.Language = updatedCollection.Language;

            if (!string.IsNullOrEmpty(updatedCollection.UserId) && updatedCollection.UserId != existingCollection.UserId)
                existingCollection.UserId = updatedCollection.UserId;
            
            if (updatedCollection.Words != null && updatedCollection.Words.Count > 0)
            {
                existingCollection.Words = updatedCollection.Words;
            }
            
            await _context.WordsCollections.ReplaceOneAsync(w => w.Id == collectionId, existingCollection);

            return Ok(existingCollection);
        }
        
        [HttpPut("{collectionId}/word/{wordId}")]
        public async Task<IActionResult> UpdateWordInCollection(int collectionId, string wordId, Word updatedWord)
        {
            var collection = await _context.WordsCollections.Find(w => w.Id == collectionId).FirstOrDefaultAsync();
            if (collection == null)
                return NotFound("Collection not found");

            var word = collection.Words.FirstOrDefault(w => w.WordText == wordId);
            if (word == null)
                return NotFound("Word not found");

            word.WordText = updatedWord.WordText;
            word.Description = updatedWord.Description;

            await _context.WordsCollections.ReplaceOneAsync(w => w.Id == collectionId, collection);

            return Ok(word);
        }

        [HttpPut("{collectionId}/word/{wordId}/example/{exampleId}")]
        public async Task<IActionResult> UpdateExampleForWord(int collectionId, string wordId, int exampleId, Example updatedExample)
        {
            var collection = await _context.WordsCollections.Find(w => w.Id == collectionId).FirstOrDefaultAsync();
            if (collection == null)
                return NotFound("Collection not found");

            var word = collection.Words.FirstOrDefault(w => w.WordText == wordId);
            if (word == null)
                return NotFound("Word not found");

            var example = word.Examples.FirstOrDefault(e => e.Id == exampleId);
            if (example == null)
                return NotFound("Example not found");

            example.Text = updatedExample.Text;

            await _context.WordsCollections.ReplaceOneAsync(w => w.Id == collectionId, collection);

            return Ok(example);
        }

    }
}
