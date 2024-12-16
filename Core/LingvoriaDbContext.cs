using Core.Models.LibraryModels;
using Core.Models.UserModels;
using Core.Models.UserModels.Activity;
using MongoDB.Driver;

namespace Core;

public class LingvoriaDbContext
{
    private readonly IMongoDatabase _database;

    public LingvoriaDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<WordsCollection> WordsCollections =>
        _database.GetCollection<WordsCollection>("WordsCollections");

    public IMongoCollection<Word> Words => _database.GetCollection<Word>("Words");
    public IMongoCollection<Example> Examples => _database.GetCollection<Example>("Examples");
    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    public IMongoCollection<UserLog> UserLogs => _database.GetCollection<UserLog>("UserLogs");
    public IMongoCollection<EmailCode> EmailCodes => _database.GetCollection<EmailCode>("EmailCodes");
}