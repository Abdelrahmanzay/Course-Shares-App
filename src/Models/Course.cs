using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CourseSharesApp.Models
{
    public class Course
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("title")] public string Title { get; set; }
        [BsonElement("code")] public string Code { get; set; }
        [BsonElement("description")] public string Description { get; set; }

        [BsonElement("section_id")] public ObjectId SectionId { get; set; }
    }
}
