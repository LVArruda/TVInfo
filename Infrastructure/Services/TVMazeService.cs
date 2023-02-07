using System.Text.Json;
using System.Net;
using Infrastructure.Interfaces;
using Models;

namespace Infrastructure.Services
{
    public class TVMazeService : ITVMazeService
    {
        private readonly HttpClient _httpClient;

        public TVMazeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<TVShow>> GetTVshowsPerPage(int page)
        {
            var tvShows = new List<TVShow>();

            var tvShowsResult = await _httpClient.GetAsync($"shows?page={page}");

            if (tvShowsResult.StatusCode == HttpStatusCode.NotFound)
            {
                return tvShows;
            }
            IEnumerable<TVShow>? tvShowsToAdd = await Deserialize<IEnumerable<TVShow>>(tvShowsResult);

            await AddCastData(tvShows, tvShowsToAdd);

            return tvShows;
        }

        private async Task AddCastData(List<TVShow> tvShows, IEnumerable<TVShow>? tvShowsToAdd)
        {
            foreach (var tvShow in tvShowsToAdd)
            {
                var castResult = await _httpClient.GetAsync($"/shows/{tvShow.Id}/cast");

                if (castResult.IsSuccessStatusCode)
                {
                    var cast = await Deserialize<IEnumerable<TVMazeCastDTO>>(castResult);

                    tvShow.Cast = cast.Select(c => new CastMember() { Birthday = c.Person.Birthday is not null ? DateTime.Parse(c.Person.Birthday) : null, Id = c.Person.Id, Name = c.Person.Name });
                }
            }

            tvShows.AddRange(tvShowsToAdd);
        }

        private static async Task<T> Deserialize<T>(HttpResponseMessage response)
        {
            return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private class TVMazeCastDTO
        {
            public TVMazePersonDTO Person { get; set; }

        }

        private class TVMazePersonDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Birthday { get; set; }
        }
    }
}
