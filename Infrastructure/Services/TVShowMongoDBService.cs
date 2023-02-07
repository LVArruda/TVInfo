using Infrastructure.Interfaces;
using Models;

namespace Infrastructure.Services
{
    public class TVShowMongoDBService : ITVShowMongoDBService
    {
        //TODO: Reason I've decided to create this service is because the recommended code from Microsoft and MongoDB on how to use Mongo is not easy to test,
        //i.e. lot of dependencies that I am not familiar with.

        private readonly IMongoDBClient _mongoDBService;

        public TVShowMongoDBService(IMongoDBClient mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        public async Task<IEnumerable<TVShow>> GetAsync(int page)
        {
            return await _mongoDBService.GetAsync(page);
        }

        public async Task UpdateManyAsync(IEnumerable<TVShow> tvShows)
        {
            foreach (var tvShow in tvShows)
            {
                tvShow.Cast = tvShow.Cast.OrderByDescending(c => c.Birthday ?? DateTime.MinValue);
            }

            await _mongoDBService.UpdateAsync(tvShows);

            return;
        }
    }
}
