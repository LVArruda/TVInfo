using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace Models
{
    public class TVShow
    {
        public TVShow()
        {
            Cast = new List<CastMember>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string? DocumentId { get; set; }
        public int Id { get; set; }
        public string? Name { get; set; }

        public IEnumerable<CastMember> Cast { get; set; }
    }
}