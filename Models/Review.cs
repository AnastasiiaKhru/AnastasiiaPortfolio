using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AnastasiiaPortfolio.Models
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("email")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("rating")]
        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [BsonElement("comment")]
        [Required(ErrorMessage = "Comment is required")]
        [StringLength(500, ErrorMessage = "Comment cannot be longer than 500 characters")]
        public string Comment { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("isVerified")]
        public bool IsVerified { get; set; }

        [BsonElement("isFeatured")]
        public bool IsFeatured { get; set; }

        [BsonElement("helpfulCount")]
        public int HelpfulCount { get; set; }

        public int NotHelpfulCount { get; set; }

        public ReviewSortOption SortOption { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsHidden { get; set; }

        public int? ProjectId { get; set; }

        public string? UserId { get; set; }

        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(500)]
        public string? Pros { get; set; }

        [StringLength(500)]
        public string? Cons { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project? Project { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<ReviewVote> Votes { get; set; } = new List<ReviewVote>();
    }

    public enum ReviewSortOption
    {
        Newest,
        HighestRated,
        LowestRated,
        MostHelpful,
        MostControversial
    }
} 