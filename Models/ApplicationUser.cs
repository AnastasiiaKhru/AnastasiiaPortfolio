using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnastasiiaPortfolio.Models
{
    public class ApplicationUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userName")]
        [Required]
        public string UserName { get; set; } = string.Empty;

        [BsonElement("normalizedUserName")]
        public string NormalizedUserName { get; set; } = string.Empty;

        [BsonElement("email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [BsonElement("normalizedEmail")]
        public string NormalizedEmail { get; set; } = string.Empty;

        [BsonElement("emailConfirmed")]
        public bool EmailConfirmed { get; set; }

        [BsonElement("passwordHash")]
        public string? PasswordHash { get; set; }

        [BsonElement("securityStamp")]
        public string? SecurityStamp { get; set; }

        [BsonElement("concurrencyStamp")]
        public string? ConcurrencyStamp { get; set; }

        [BsonElement("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [BsonElement("phoneNumberConfirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        [BsonElement("twoFactorEnabled")]
        public bool TwoFactorEnabled { get; set; }

        [BsonElement("lockoutEnd")]
        public DateTimeOffset? LockoutEnd { get; set; }

        [BsonElement("lockoutEnabled")]
        public bool LockoutEnabled { get; set; }

        [BsonElement("accessFailedCount")]
        public int AccessFailedCount { get; set; }

        [BsonElement("firstName")]
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [BsonElement("lastName")]
        [Required]
        public string LastName { get; set; } = string.Empty;

        [BsonElement("isAdmin")]
        public bool IsAdmin { get; set; }

        [BsonElement("role")]
        public string Role { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("reviews")]
        public List<Review> Reviews { get; set; } = new List<Review>();

        [BsonElement("reviewVotes")]
        public List<ReviewVote> ReviewVotes { get; set; } = new List<ReviewVote>();
    }
} 