using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AnastasiiaPortfolio.Models
{
    public class Project
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("title")]
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        [Required]
        public string Description { get; set; } = string.Empty;

        [BsonElement("imageUrl")]
        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [BsonElement("projectUrl")]
        public string? ProjectUrl { get; set; }

        [BsonElement("githubUrl")]
        public string? GitHubUrl { get; set; }

        [BsonElement("category")]
        [Required]
        public string Category { get; set; } = string.Empty;

        [BsonElement("technologies")]
        [Required]
        public string Technologies { get; set; } = string.Empty;

        [BsonElement("isFeatured")]
        public bool IsFeatured { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("dateCompleted")]
        public DateTime DateCompleted { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("reviews")]
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
} 