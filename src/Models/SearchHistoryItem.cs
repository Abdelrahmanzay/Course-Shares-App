using System;
using MongoDB.Bson.Serialization.Attributes;

namespace CourseSharesApp.Models
{
    [BsonIgnoreExtraElements]
    public class SearchHistoryItem
    {
        [BsonElement("keyword")] public string Keyword { get; set; }
        [BsonElement("timestamp")] public DateTime Timestamp { get; set; }
    }
}
