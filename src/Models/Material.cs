using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CourseSharesApp.Models
{
    [BsonIgnoreExtraElements]
    public class Material
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("title")] public string Title { get; set; }
        [BsonElement("type")] public string Type { get; set; } // file/link

        // ADDED THIS: It was missing but exists in your script
        [BsonElement("fileLink")] public string FileLink { get; set; }

        // FIXED: name matches phase2.js (uploadDate)
        [BsonElement("uploadDate")] public DateTime UploadDate { get; set; }

        // FIXED: name matches phase2.js (status)
        [BsonElement("status")] public string Status { get; set; }

        // FIXED: name matches phase2.js (viewsCount)
        [BsonElement("viewsCount")] public int ViewsCount { get; set; }

        // FIXED: name matches phase2.js (uploadedBy)
        [BsonElement("uploadedBy")] public ObjectId UserId { get; set; }

        // FIXED: name matches phase2.js (courseId)
        [BsonElement("courseId")] public ObjectId CourseId { get; set; }

        [BsonElement("comments")] public List<Comment> Comments { get; set; }
    }
}