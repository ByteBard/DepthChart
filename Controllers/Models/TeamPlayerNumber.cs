using MongoDB.Bson.Serialization.Attributes;

namespace DepthChart01.Controllers.Models
{
    [Serializable, BsonIgnoreExtraElements]
    public class TeamPlayerNumber
    {
        [BsonElement("team_id"), BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string TeamId { get; set; }

        [BsonElement("number"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string Number { get; set; }
    }
}
