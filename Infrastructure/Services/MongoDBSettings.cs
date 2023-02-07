namespace Infrastructure.Services
{
    public class MongoDBSettings
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string TvShowCollectionName { get; set; } = null!;
    }
}
