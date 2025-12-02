using MongoDB.Bson.Serialization.Attributes;

namespace CourseSharesApp.Models
{
    public class Address
    {
        [BsonElement("city")] public string City { get; set; }
        [BsonElement("country")] public string Country { get; set; }
        [BsonElement("postal_code")] public string PostalCode { get; set; }
    }
}
