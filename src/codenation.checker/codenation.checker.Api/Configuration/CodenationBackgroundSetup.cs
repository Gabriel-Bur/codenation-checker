using codenation.checker.Api.Interfaces;
using codenation.checker.Api.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace codenation.checker.Api.Configuration
{
    public class CodenationBackgroundSetup : IDisposable, IHostedService
    {
        private const int RunIntervalInMinutes = 59;

        private readonly ICodenationApiClientService _codenationApiClient;
        private readonly ILogger _logger;
        private Timer _timer;

        public CodenationBackgroundSetup(
            ILogger<CodenationBackgroundService> logger,
            ICodenationApiClientService codenationApiClient)
        {
            _logger = logger;
            _codenationApiClient = codenationApiClient;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");

            _codenationApiClient.Execute();
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(RunIntervalInMinutes));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
