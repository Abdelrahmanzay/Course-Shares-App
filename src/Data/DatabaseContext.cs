using MongoDB.Driver;
using CourseSharesApp.Models;

namespace CourseSharesApp.Data
{
    public class DatabaseContext
    {
        private readonly IMongoDatabase _database;

        public DatabaseContext()
        {
            var connectionString = "mongodb+srv://Agent:CyO41ftEO2jYc3Jf@agents.jfuv468.mongodb.net/";
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("CourseShares");
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
        public IMongoCollection<Material> Materials => _database.GetCollection<Material>("materials");
        public IMongoCollection<Course> Courses => _database.GetCollection<Course>("courses");
        public IMongoCollection<Section> Sections => _database.GetCollection<Section>("sections");
        public IMongoCollection<SearchHistoryItem> SearchHistory => _database.GetCollection<SearchHistoryItem>("searchHistory");
    }
}