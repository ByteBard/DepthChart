using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DepthChart01.Controllers.Models
{
    [Serializable, BsonIgnoreExtraElements]
    public class Team
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string TeamId { get; set; }

        [BsonElement("category"), BsonRepresentation(BsonType.String)]
        public string Category { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string Name { get; set; }
    }
}
