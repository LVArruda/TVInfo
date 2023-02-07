using Models;

namespace Infrastructure.Interfaces
{
    public interface ITVShowMongoDBService
    {
        public Task<IEnumerable<TVShow>> GetAsync(int page);
        public Task UpdateManyAsync(IEnumerable<TVShow> tvShows);
    }
}
