using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DepthChart01.Controllers.Models
{
    public class TeamPosition
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string TeamPositionId { get; set; }

        [BsonElement("team_id"), BsonRepresentation(BsonType.ObjectId)]
        public string TeamId { get; set; }

        [BsonElement("position_name"), BsonRepresentation(BsonType.String)]
        public string Name { get; set; }

        [BsonElement("player_positions")]
        public PlayerPosition[] PlayerPositions { get; set; }
    }
}
