using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CourseSharesApp.Models
{
    public class User
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("name")] public string Name { get; set; }
        [BsonElement("email")] public string Email { get; set; }
        [BsonElement("password")] public string Password { get; set; }
        [BsonElement("role")] public string Role { get; set; }
        [BsonElement("date_joined")] public DateTime DateJoined { get; set; }

        [BsonElement("address")] public Address Address { get; set; }

        [BsonElement("search_history")] public List<SearchHistoryItem> SearchHistory { get; set; }
    }
}
