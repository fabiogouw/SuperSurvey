using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SuperSurvey.UseCases.Ports.In;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SuperSurvey.WebApp.HostedServices;
public class SQSVoteCounterHostedService : IHostedService, IDisposable
{
    private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
    private readonly SQSVoteCounterHandler _handler;
    private readonly ILogger<SQSVoteCounterHostedService> _logger;
    private Timer _timer;

    public SQSVoteCounterHostedService(ILogger<SQSVoteCounterHostedService> logger,
        IAmazonSQS client,
        CountVotesUseCase countVotesUseCase,
        IConfiguration configuration)
    {
        _logger = logger;
        _handler = new SQSVoteCounterHandler(client, countVotesUseCase, configuration.GetConnectionString("VoteQueue"));
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(async (state) =>
        {
            try
            {
                int messagesProcessed = await _handler.Execute();
                _logger.LogInformation($"Checked for new items: { messagesProcessed } processed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }
}
