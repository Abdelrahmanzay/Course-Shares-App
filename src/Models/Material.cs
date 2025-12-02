using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CourseSharesApp.Models
{
    public class Material
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("title")] public string Title { get; set; }
        [BsonElement("type")] public string Type { get; set; } // file/link
        [BsonElement("upload_date")] public DateTime UploadDate { get; set; }
        [BsonElement("status")] public string Status { get; set; } // Pending/Approved
        [BsonElement("views_count")] public int ViewsCount { get; set; }

        [BsonElement("user_id")] public ObjectId UserId { get; set; }
        [BsonElement("course_id")] public ObjectId CourseId { get; set; }

        [BsonElement("comments")] public List<Comment> Comments { get; set; }
    }
}
