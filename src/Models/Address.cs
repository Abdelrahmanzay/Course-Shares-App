using MongoDB.Bson.Serialization.Attributes;

namespace CourseSharesApp.Models
{
    [BsonIgnoreExtraElements]
    public class Address
    {
        [BsonElement("city")] public string City { get; set; }
        [BsonElement("country")] public string Country { get; set; }
        [BsonElement("postalCode")] public string PostalCode { get; set; }
    }
}
