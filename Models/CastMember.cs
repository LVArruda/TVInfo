using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace Models
{
    public class CastMember
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string? DocumentId { get; set; }
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
