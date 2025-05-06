namespace AnastasiiaPortfolio.Models
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string ReviewsCollectionName { get; set; } = string.Empty;
        public string ProjectsCollectionName { get; set; } = string.Empty;
        public string UsersCollectionName { get; set; } = string.Empty;
        public string RatingsCollectionName { get; set; } = string.Empty;
        public string PlayerScoresCollectionName { get; set; } = string.Empty;
        public string ReviewVotesCollectionName { get; set; } = string.Empty;
    }
} 