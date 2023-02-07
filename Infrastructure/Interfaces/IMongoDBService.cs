using Models;

namespace Infrastructure.Interfaces
{
    public interface IMongoDBClient
    {
        public Task<IEnumerable<TVShow>> GetAsync(int page);
        public Task UpdateAsync(IEnumerable<TVShow> tvShows);
    }
}
