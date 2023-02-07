using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Services
{
    public class MongoDBClient : IMongoDBClient
    {

        private readonly IMongoCollection<TVShow> _tvShowCollection;

        public MongoDBClient(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _tvShowCollection = database.GetCollection<TVShow>(mongoDBSettings.Value.TvShowCollectionName);
        }

        public async Task<List<TVShow>> GetAsync(int page)
        {
            var pageSize = 50;

            return await _tvShowCollection.Find(new BsonDocument())
                                          .Skip(page * pageSize).Limit(pageSize).ToListAsync();
        }
        public async Task UpdateAsync(IEnumerable<TVShow> tvShows)
        {
            var bulkList = new List<UpdateOneModel<TVShow>>();

            foreach (var tvShow in tvShows)
            {
                var model = new UpdateOneModel<TVShow>(
                Builders<TVShow>.Filter.Eq("Id", tvShow.Id),
                Builders<TVShow>.Update.Set("Cast", tvShow.Cast)
                                                    .Set("Name", tvShow.Name)
                                                    );
                model.IsUpsert = true;

                bulkList.Add(model);
            }
            
            await _tvShowCollection.BulkWriteAsync(bulkList);

            return;
        }
    }
}
