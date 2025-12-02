using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CourseSharesApp.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")] public string Name { get; set; }
        [BsonElement("email")] public string Email { get; set; }

        // FIXED: script uses passwordHash
        [BsonElement("passwordHash")] public string Password { get; set; }

        [BsonElement("role")] public string Role { get; set; }

        // FIXED: script uses dateJoined
        [BsonElement("dateJoined")] public DateTime DateJoined { get; set; }

        [BsonElement("address")] public Address Address { get; set; }
        [BsonElement("searchHistory")] public List<SearchHistoryItem> SearchHistory { get; set; }
    }
}