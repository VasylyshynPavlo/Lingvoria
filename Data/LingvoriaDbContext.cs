using MongoDB.Driver;
using Data.Models;

namespace Data
{
    public class LingvoriaDbContext
    {
        private readonly IMongoDatabase _database;

        public LingvoriaDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<WordsCollection> WordsCollections => _database.GetCollection<WordsCollection>("WordsCollections");
        public IMongoCollection<Word> Words => _database.GetCollection<Word>("Words");
        public IMongoCollection<Example> Examples => _database.GetCollection<Example>("Examples");
    }
}