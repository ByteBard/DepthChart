using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DepthChart01.Controllers.Models
{
    [Serializable, BsonIgnoreExtraElements]
    public class Position
    {
        [BsonElement("player_id"), BsonRepresentation(BsonType.ObjectId)]
        public string PlayerId { get; set; }

        [BsonElement("order"), BsonRepresentation(BsonType.Int32)]
        public int Order { get; set; }
    }
}
