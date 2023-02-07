using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using TVInfo.Console.Scraper;

namespace MyConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                             .ConfigureServices((hostContext, services) =>
                             {
                                 var configuration = hostContext.Configuration;

                                 AddHttpClient(services, configuration);

                                 services.Configure<MongoDBSettings>(configuration.GetSection("MongoDB"));

                                 /*
                                 Per the official Mongo Client reuse guidelines, MongoClient should be registered in DI with a singleton service lifetime.
                                 https://mongodb.github.io/mongo-csharp-driver/2.14/reference/driver/connecting/#re-use
                                 */
                                 services.AddSingleton<IMongoDBClient, MongoDBClient>();
                                 services.AddTransient<ITVShowMongoDBService, TVShowMongoDBService>();
                                 services.AddHostedService<Worker>();
                             })
                             .ConfigureLogging(((hostingContext, logging) =>
                             {
                                 logging.ClearProviders();
                                 logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                                 logging.AddConsole(options =>
                                 {
                                     options.TimestampFormat = "HH:mm:ss ";
                                 });
                                 logging.AddDebug();
                             }))
                             .Build();

            host.Run();
        }

        private static void AddHttpClient(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<ITVMazeService, TVMazeService>(
                                                    client =>
                                                    {
                                                        //TODO: Additionally, you should ensure that HTTP connections are not unnecessarily left open. Either explicitly close the HTTP connection after every request, re-use connections for subsequent requests, or make use of HTTP2 multiplexing
                                                        client.BaseAddress = new Uri(configuration["TVMazeBaseUrl"]);
                                                        client.DefaultRequestHeaders.UserAgent.ParseAdd("Scraper-Leandro");
                                                    })
                                                    .AddPolicyHandler(GetRetryPolicy());
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}