using Infrastructure.Interfaces;
using Infrastructure.Services;
using Models;
using Moq;
using System.Data;

namespace TVInfo.UnitTests
{
    public class TVShowMongoDBServiceTests
    {
        private readonly Mock<IMongoDBClient> _mongoDBServiceMock;

        public TVShowMongoDBServiceTests()
        {
            _mongoDBServiceMock = new Mock<IMongoDBClient>();
        }

        [Fact]
        public async void TVShowMongoDBService_Upserts_WithCastSortedByBirthday()
        {
            IEnumerable<TVShow> callbackTVShows = new List<TVShow>();

            List<CastMember> castToBeInserted = new List<CastMember> { new CastMember { Id = 1, Name = "Name1", Birthday = DateTime.Parse("20/01/2000") },
                                                           new CastMember { Id = 2, Name = "Name2", Birthday = DateTime.Parse("20/01/1960") }};

            List<CastMember> expectedCast = castToBeInserted.OrderByDescending(c => c.Birthday ?? DateTime.MinValue).ToList();

            List<TVShow> tVShowsToBeInserted = new List<TVShow>() { new TVShow() { Id = 1, Name = "TVShow1", Cast = castToBeInserted } };

            _mongoDBServiceMock
            .Setup(c => c.UpdateAsync(It.IsAny<IEnumerable<TVShow>>()))
            .Callback<IEnumerable<TVShow>>((obj) => callbackTVShows = obj);

            var sut = new TVShowMongoDBService(_mongoDBServiceMock.Object);

            await sut.UpdateManyAsync(tVShowsToBeInserted);

            Assert.Equal(expectedCast[0], callbackTVShows.First().Cast.ToList()[0]);
        }
    }
}