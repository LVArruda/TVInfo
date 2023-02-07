using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Models;
using System.Net;
using System.Text.Json;

namespace TVInfo.API.IntegrationTests
{
    public class TVShowControllerTests :
    IClassFixture<TVInfoAPICustomApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TVShowControllerTests(TVInfoAPICustomApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetTVShows_BringsResults_EvenWhenNoPageParameter()
        {
            var response = await _client.GetAsync("api/TVShow");
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Length.Should().NotBe(0);
        }

        [Fact]
        public async Task GetTVShows_Returns_ExpectedJson()
        {
            var expectedJsonString = ExpectedJsonStrings.TVShowGetWithoutPage();

            var response = await _client.GetAsync("api/TVShow");
            var responseJsonString = (await response.Content.ReadAsStringAsync());

            responseJsonString.Should().Be(expectedJsonString);
        }

        [Fact]
        public async Task GetTVShows_Returns_50PaginatedResults()
        {
            var pageSize = 50;
            var responsePage0 = await _client.GetAsync("api/TVShow");
            var responsePage1 = await _client.GetAsync("api/TVShow?Page=1");
            var responseJsonStringPage0 = (await responsePage0.Content.ReadAsStringAsync());
            var responseJsonStringPage1 = (await responsePage1.Content.ReadAsStringAsync());

            var tvShowsPage0 = JsonSerializer.Deserialize<IEnumerable<TVShow>>(responseJsonStringPage0, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var tvShowsPage1 = JsonSerializer.Deserialize<IEnumerable<TVShow>>(responseJsonStringPage1, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            responseJsonStringPage0.Should().NotBe(responseJsonStringPage1);
            tvShowsPage0.Count().Should().Be(pageSize);
            tvShowsPage1.Count().Should().Be(pageSize);
            tvShowsPage0.Last().Id.Should().BeLessThan(tvShowsPage1.First().Id);
        }

        [Fact]
        public async Task GetTVShows_Returns_TVShowWithCastOrderedByBirthday()
        {
            var response = await _client.GetAsync("api/TVShow");
     
            var tvShows = JsonSerializer.Deserialize<IEnumerable<TVShow>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var someTVShow1 = tvShows.First();
            var someTVShow2 = tvShows.Last();

            var orderedCast1 = someTVShow1.Cast.OrderByDescending(c => c.Birthday);
            var orderedCast2 = someTVShow2.Cast.OrderByDescending(c => c.Birthday);

            someTVShow1.Cast.Should().BeEquivalentTo(orderedCast1, options => options.WithStrictOrdering());
            someTVShow2.Cast.Should().BeEquivalentTo(orderedCast2, options => options.WithStrictOrdering());
        }
    }
}