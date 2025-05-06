using Microsoft.Extensions.Options;
using MongoDB.Driver;
using AnastasiiaPortfolio.Models;
using Microsoft.Extensions.Logging;

namespace AnastasiiaPortfolio.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Review> _reviewsCollection;
        private readonly IMongoCollection<Project> _projectsCollection;
        private readonly IMongoCollection<ApplicationUser> _usersCollection;
        private readonly IMongoCollection<Rating> _ratingsCollection;
        private readonly IMongoCollection<PlayerScore> _playerScoresCollection;
        private readonly IMongoCollection<ReviewVote> _reviewVotesCollection;
        private readonly IMongoDatabase _database;
        private readonly MongoClient _client;
        private readonly ILogger<MongoDBService> _logger;

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings, ILogger<MongoDBService> logger)
        {
            _logger = logger;
            try
            {
                _logger.LogInformation("Starting MongoDB connection initialization...");
                
                var settings = MongoClientSettings.FromConnectionString(mongoDBSettings.Value.ConnectionString);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);
                
                _logger.LogInformation("Initializing MongoDB client...");
                _client = new MongoClient(settings);

                _logger.LogInformation("Getting database: {DatabaseName}", mongoDBSettings.Value.DatabaseName);
                _database = _client.GetDatabase(mongoDBSettings.Value.DatabaseName);
                
                _logger.LogInformation("Testing connection with ping command...");
                var pingCommand = new MongoDB.Bson.BsonDocument("ping", 1);
                _database.RunCommand<MongoDB.Bson.BsonDocument>(pingCommand);
                
                _logger.LogInformation("Initializing collections...");
                _reviewsCollection = _database.GetCollection<Review>(mongoDBSettings.Value.ReviewsCollectionName);
                _projectsCollection = _database.GetCollection<Project>(mongoDBSettings.Value.ProjectsCollectionName);
                _usersCollection = _database.GetCollection<ApplicationUser>(mongoDBSettings.Value.UsersCollectionName);
                _ratingsCollection = _database.GetCollection<Rating>(mongoDBSettings.Value.RatingsCollectionName);
                _playerScoresCollection = _database.GetCollection<PlayerScore>(mongoDBSettings.Value.PlayerScoresCollectionName);
                _reviewVotesCollection = _database.GetCollection<ReviewVote>(mongoDBSettings.Value.ReviewVotesCollectionName);
                
                _logger.LogInformation("Successfully connected to MongoDB and initialized all collections");
            }
            catch (MongoAuthenticationException authEx)
            {
                _logger.LogError(authEx, "MongoDB authentication failed. Error details: {Message}", authEx.Message);
                throw;
            }
            catch (MongoConnectionException connEx)
            {
                _logger.LogError(connEx, "Failed to connect to MongoDB. Error details: {Message}", connEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error initializing MongoDB connection. Error details: {Message}", ex.Message);
                throw;
            }
        }

        public IMongoDatabase GetDatabase() => _database;
        
        public MongoClient GetMongoClient() => _client;

        // Reviews
        public async Task<List<Review>> GetReviewsAsync(ReviewSortOption sortOption = ReviewSortOption.Newest)
        {
            var sort = sortOption switch
            {
                ReviewSortOption.Newest => Builders<Review>.Sort.Descending(r => r.CreatedAt),
                ReviewSortOption.HighestRated => Builders<Review>.Sort.Descending(r => r.Rating),
                ReviewSortOption.LowestRated => Builders<Review>.Sort.Ascending(r => r.Rating),
                ReviewSortOption.MostHelpful => Builders<Review>.Sort.Descending(r => r.HelpfulCount),
                _ => Builders<Review>.Sort.Descending(r => r.CreatedAt)
            };

            return await _reviewsCollection.Find(review => !review.IsHidden)
                .Sort(sort)
                .ToListAsync();
        }

        public async Task<Review> GetReviewAsync(string id) =>
            await _reviewsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateReviewAsync(Review review)
        {
            review.CreatedAt = DateTime.UtcNow;
            review.IsVerified = false;
            review.IsFeatured = false;
            review.HelpfulCount = 0;
            await _reviewsCollection.InsertOneAsync(review);
        }

        public async Task UpdateReviewAsync(string id, Review review)
        {
            review.UpdatedAt = DateTime.UtcNow;
            await _reviewsCollection.ReplaceOneAsync(x => x.Id == id, review);
        }

        public async Task DeleteReviewAsync(string id) =>
            await _reviewsCollection.DeleteOneAsync(x => x.Id == id);

        public async Task UpdateReviewHelpfulCountAsync(string id, bool isHelpful)
        {
            var update = isHelpful
                ? Builders<Review>.Update.Inc(r => r.HelpfulCount, 1)
                : Builders<Review>.Update.Inc(r => r.NotHelpfulCount, 1);

            await _reviewsCollection.UpdateOneAsync(x => x.Id == id, update);
        }

        public async Task ToggleReviewVerificationAsync(string id)
        {
            var review = await GetReviewAsync(id);
            if (review != null)
            {
                review.IsVerified = !review.IsVerified;
                await UpdateReviewAsync(id, review);
            }
        }

        public async Task ToggleReviewFeaturedAsync(string id)
        {
            var review = await GetReviewAsync(id);
            if (review != null)
            {
                review.IsFeatured = !review.IsFeatured;
                await UpdateReviewAsync(id, review);
            }
        }

        // Projects
        public async Task<List<Project>> GetProjectsAsync() =>
            await _projectsCollection.Find(_ => true).ToListAsync();

        public async Task<Project> GetProjectAsync(string id) =>
            await _projectsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateProjectAsync(Project project)
        {
            project.CreatedAt = DateTime.UtcNow;
            await _projectsCollection.InsertOneAsync(project);
        }

        public async Task UpdateProjectAsync(string id, Project project) =>
            await _projectsCollection.ReplaceOneAsync(x => x.Id == id, project);

        public async Task DeleteProjectAsync(string id) =>
            await _projectsCollection.DeleteOneAsync(x => x.Id == id);

        // Users
        public async Task<List<ApplicationUser>> GetUsersAsync() =>
            await _usersCollection.Find(_ => true).ToListAsync();

        public async Task<ApplicationUser> GetUserAsync(string id) =>
            await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<ApplicationUser> GetUserByEmailAsync(string email) =>
            await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

        public async Task CreateUserAsync(ApplicationUser user)
        {
            user.CreatedAt = DateTime.UtcNow;
            await _usersCollection.InsertOneAsync(user);
        }

        public async Task UpdateUserAsync(string id, ApplicationUser user) =>
            await _usersCollection.ReplaceOneAsync(x => x.Id == id, user);

        public async Task DeleteUserAsync(string id) =>
            await _usersCollection.DeleteOneAsync(x => x.Id == id);

        // Ratings
        public async Task<List<Rating>> GetRatingsAsync() =>
            await _ratingsCollection.Find(_ => true).ToListAsync();

        public async Task<Rating> GetRatingAsync(string id) =>
            await _ratingsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateRatingAsync(Rating rating)
        {
            rating.CreatedAt = DateTime.UtcNow;
            await _ratingsCollection.InsertOneAsync(rating);
        }

        public async Task UpdateRatingAsync(string id, Rating rating) =>
            await _ratingsCollection.ReplaceOneAsync(x => x.Id == id, rating);

        public async Task DeleteRatingAsync(string id) =>
            await _ratingsCollection.DeleteOneAsync(x => x.Id == id);

        // PlayerScores
        public async Task<List<PlayerScore>> GetPlayerScoresAsync() =>
            await _playerScoresCollection.Find(_ => true)
                .SortByDescending(x => x.Score)
                .ToListAsync();

        public async Task<List<PlayerScore>> GetTopPlayerScoresAsync(int count) =>
            await _playerScoresCollection.Find(_ => true)
                .SortByDescending(x => x.Score)
                .Limit(count)
                .ToListAsync();

        public async Task<PlayerScore> GetPlayerScoreAsync(string id) =>
            await _playerScoresCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreatePlayerScoreAsync(PlayerScore score)
        {
            score.PlayedAt = DateTime.UtcNow;
            await _playerScoresCollection.InsertOneAsync(score);
        }

        public async Task UpdatePlayerScoreAsync(string id, PlayerScore score) =>
            await _playerScoresCollection.ReplaceOneAsync(x => x.Id == id, score);

        public async Task DeletePlayerScoreAsync(string id) =>
            await _playerScoresCollection.DeleteOneAsync(x => x.Id == id);

        // ReviewVotes
        public async Task<List<ReviewVote>> GetReviewVotesAsync() =>
            await _reviewVotesCollection.Find(_ => true).ToListAsync();

        public async Task<ReviewVote> GetReviewVoteAsync(string id) =>
            await _reviewVotesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<ReviewVote> GetReviewVoteByUserAndReviewAsync(string userId, string reviewId) =>
            await _reviewVotesCollection.Find(x => x.UserId == userId && x.ReviewId == reviewId).FirstOrDefaultAsync();

        public async Task CreateReviewVoteAsync(ReviewVote vote)
        {
            vote.CreatedAt = DateTime.UtcNow;
            await _reviewVotesCollection.InsertOneAsync(vote);
        }

        public async Task UpdateReviewVoteAsync(string id, ReviewVote vote) =>
            await _reviewVotesCollection.ReplaceOneAsync(x => x.Id == id, vote);

        public async Task DeleteReviewVoteAsync(string id) =>
            await _reviewVotesCollection.DeleteOneAsync(x => x.Id == id);
    }
} 