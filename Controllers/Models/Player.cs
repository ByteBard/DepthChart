using MongoDB.Bson.Serialization.Attributes;

namespace DepthChart01.Controllers.Models
{
    [Serializable, BsonIgnoreExtraElements]
    public class Player
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string PlayerId { get; set; }

        [BsonElement("current_team_id"), BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string CurrentTeamId { get; set; }

        [BsonElement("name"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string Name { get; set; }

        [BsonElement("team_player_numbers")]
        public TeamPlayerNumber[] TeamPlayerNumbers { get; set; }
    }
}
