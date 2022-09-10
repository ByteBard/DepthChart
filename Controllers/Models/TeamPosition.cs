using MongoDB.Bson.Serialization.Attributes;

namespace DepthChart01.Controllers.Models
{
    public class TeamPosition
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string TeamPositionId { get; set; }

        [BsonElement("team_id"), BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string TeamId { get; set; }

        [BsonElement("positions")]
        public Position[] Positions { get; set; }
    }
}
