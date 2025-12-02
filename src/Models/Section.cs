using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CourseSharesApp.Models
{
    public class Section
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("section_name")] public string SectionName { get; set; }
        [BsonElement("description")] public string Description { get; set; }
    }
}
