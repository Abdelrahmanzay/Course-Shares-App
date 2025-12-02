using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CourseSharesApp.Models
{
    public class Comment
    {
        [BsonElement("text")] public string Text { get; set; }
        [BsonElement("user_id")] public ObjectId UserId { get; set; }
        [BsonElement("timestamp")] public DateTime Timestamp { get; set; }
    }
}
