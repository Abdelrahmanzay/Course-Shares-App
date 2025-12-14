using MongoDB.Driver;
using MongoDB.Driver.GridFS; 
using CourseSharesApp.Models;
using System.Threading.Tasks;

namespace CourseSharesApp.Data
{
    public class DatabaseContext
    {
        private readonly IMongoDatabase _database;

    
        public IGridFSBucket Bucket { get; private set; }

        public DatabaseContext()
        {
            var connectionString = "mongodb+srv://Agent:nDxPoMdFNw4eadaG@agents.jfuv468.mongodb.net/CourseShares";
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("CourseShares");

            
            Bucket = new GridFSBucket(_database);
        }


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