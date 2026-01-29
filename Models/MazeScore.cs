using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AnastasiiaPortfolio.Models
{
    public class MazeScore
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        [StringLength(50)]
        [BsonElement("playerName")]
        public string PlayerName { get; set; } = string.Empty;

        [BsonElement("timeSeconds")]
        public int TimeSeconds { get; set; }

        [BsonElement("mazeSize")]
        public int MazeSize { get; set; } = 10;

        [BsonElement("playedAt")]
        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
    }
}
