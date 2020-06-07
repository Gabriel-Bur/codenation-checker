using codenation.checker.Api.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace codenation.checker.Api.Services
{
    public class CodenationBackgroundService : IHostedService, IDisposable
    {
        private readonly ICodenationApiClient _codenationApiClient;
        private readonly ILogger _logger;
        private Timer _timer;

        public CodenationBackgroundService(
            ILogger<CodenationBackgroundService> logger,
            ICodenationApiClient codenationApiClient)
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

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(30));

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
