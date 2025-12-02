using System;
using MongoDB.Bson;
using MongoDB.Driver;
using CourseSharesApp.Models;

namespace CourseSharesApp.Data
{
    public class DatabaseContext
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;

        public IMongoCollection<User> Users { get; }
        public IMongoCollection<Material> Materials { get; }
        public IMongoCollection<Course> Courses { get; }
        public IMongoCollection<Section> Sections { get; }

        public DatabaseContext(string connectionString, string databaseName = "CourseShares")
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            settings.ConnectTimeout = TimeSpan.FromSeconds(5);

            _client = new MongoClient(settings);
            _db = _client.GetDatabase(databaseName);

            Users = _db.GetCollection<User>("users");
            Materials = _db.GetCollection<Material>("materials");
            Courses = _db.GetCollection<Course>("courses");
            Sections = _db.GetCollection<Section>("sections");
        }

        public bool CanConnect()
        {
            try
            {
                _db.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}