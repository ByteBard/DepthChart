﻿using MongoDB.Bson.Serialization.Attributes;

namespace DepthChart01.Controllers.Models
{
    [Serializable, BsonIgnoreExtraElements]
    public class Player
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string PlayerId { get; set; }

        [BsonElement("name"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string Name { get; set; }
    }
}