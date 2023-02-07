using Models;

namespace Infrastructure.Interfaces
{
    public interface ITVShowMongoDBService
    {
        public Task<List<TVShow>> GetAsync(int page);
        public Task UpdateManyAsync(IEnumerable<TVShow> tvShows);
    }
}
