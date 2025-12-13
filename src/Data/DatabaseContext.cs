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

        // Expose database for raw queries when needed
        public IMongoDatabase Database => _database;

        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
        public IMongoCollection<Material> Materials => _database.GetCollection<Material>("materials");
        public IMongoCollection<Course> Courses => _database.GetCollection<Course>("courses");
        public IMongoCollection<Section> Sections => _database.GetCollection<Section>("sections");
        public IMongoCollection<SearchHistoryItem> SearchHistory => _database.GetCollection<SearchHistoryItem>("searchHistory");
        
    public async Task AddSectionAsync(Section newSection)
    {
       
        var collection = _database.GetCollection<Section>("sections");


        await collection.InsertOneAsync(newSection);
    }

    public async Task AddCourseAsync(Course newCourse)
    {
        var collection = _database.GetCollection<Course>("courses");
        await collection.InsertOneAsync(newCourse);
    }
    }
}