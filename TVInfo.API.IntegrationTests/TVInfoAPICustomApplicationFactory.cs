using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TVInfo.API.IntegrationTests
{
    public class TVInfoAPICustomApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //TODO: this part we should replace database initialization with a different one, but for time saving sake I used the same database
            //I am aware that this is bad practice, the idea was just to have a fast way to have API integration (some would call acceptance/functional) tests
            //Ideally we would have a test that runs the scraper and after that check the API but this would take more time to create the tests
        }
    }
}
