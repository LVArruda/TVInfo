using Amazon.Runtime.Internal.Util;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVInfo.Console.Scraper
{
    public class Worker : BackgroundService
    {
        private readonly ITVMazeService _tvMazeService;
        private readonly ITVShowMongoDBService _tvShowMongoDBService;
        private readonly ILogger<Worker> _logger;

        public Worker(ITVMazeService tvMazeService, ITVShowMongoDBService tvShowMongoDBService, ILogger<Worker> logger)
        {
            _tvMazeService = tvMazeService;
            _tvShowMongoDBService = tvShowMongoDBService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var shows = new List<TVShow>();
            var page = 0;

            do
            {
                Stopwatch sw = Stopwatch.StartNew();
                shows = (await _tvMazeService.GetTVshowsPerPage(page)).ToList();

                if (shows.Count > 0)
                {
                    _logger.LogInformation($"*** Time elapsed getting from TVMaze API: {sw.Elapsed}");
                    page++;

                    sw.Restart();
                    await _tvShowMongoDBService.UpdateManyAsync(shows);
                    _logger.LogInformation($"*** Time elapsed to store: {sw.Elapsed}");

                    _logger.LogInformation($"Updated shows: {string.Join(",", shows.Select(s => s.Id))}");
                }                
            }
            while (shows.Count != 0 || !stoppingToken.IsCancellationRequested);
        }
    }
}
