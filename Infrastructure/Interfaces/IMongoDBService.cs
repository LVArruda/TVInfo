using Models;

namespace Infrastructure.Interfaces
{
    public interface IMongoDBClient
    {
        public Task<List<TVShow>> GetAsync(int page);
        public Task UpdateAsync(IEnumerable<TVShow> tvShows);
    }
}
