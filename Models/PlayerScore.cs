using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AnastasiiaPortfolio.Models
{
    public class PlayerScore
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("playerName")]
        [Required]
        [StringLength(50)]
        public string PlayerName { get; set; } = string.Empty;

        [BsonElement("score")]
        [Required]
        public int Score { get; set; }

        [BsonElement("playedAt")]
        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
    }
} 