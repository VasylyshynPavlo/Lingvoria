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

        public IMongoCollection<WordsCollectionModel> WordsCollections => _database.GetCollection<WordsCollectionModel>("WordsCollections");
        public IMongoCollection<WordModel> Words => _database.GetCollection<WordModel>("Words");
        public IMongoCollection<ExampleModel> Examples => _database.GetCollection<ExampleModel>("Examples");
        public IMongoCollection<UserModel> Users => _database.GetCollection<UserModel>("Users");
    }
}