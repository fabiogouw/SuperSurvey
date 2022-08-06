using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SuperSurvey.Adapters;
using SuperSurvey.UseCases.Ports.In;
using System;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Threading.Tasks;

namespace SuperSurvey.WebApp.HostedServices;
public class SQSVoteCounterHostedService : IHostedService, IDisposable
{
    private readonly SQSVoteCounterHandler _handler;
    private readonly ILogger<SQSVoteCounterHostedService> _logger;
    private Task _task;
    private volatile bool _executing = false;
    private volatile int _noMessagesCount = 0;
    private static readonly Meter _meter = new ("SuperSurveyMeter", "0.0.1");
    private static readonly Counter<long> _processedMessagesCounter = _meter.CreateCounter<long>("Processed Messages");
    private static readonly Counter<long> _executionsWithErros = _meter.CreateCounter<long>("Executions with errors");
    private static readonly Counter<long> _executionsWithNoErrors = _meter.CreateCounter<long>("Executions with no errors");

    public SQSVoteCounterHostedService(ILogger<SQSVoteCounterHostedService> logger,
        IAmazonSQS client,
        CountVotesUseCase countVotesUseCase,
        IConfiguration configuration)
    {
        _logger = logger;
        _handler = new SQSVoteCounterHandler(client, countVotesUseCase, configuration.GetConnectionString("VoteQueue"));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");
        _executing = true;
        _task = Task.Factory.StartNew(async () => await Execute(), cancellationToken);
        return Task.CompletedTask;
    }

    private async Task Execute()
    {
        while (_executing)
        {
            int messagesProcessedCount = 0;
            try
            {
                messagesProcessedCount = await _handler.Execute();
                _executionsWithNoErrors.Add(1);
                _processedMessagesCounter.Add(messagesProcessedCount);
                _logger.LogInformation($"Checked for new items: { messagesProcessedCount } processed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _executionsWithErros.Add(1);
            }
            int waitDelay = GetNextWaitDelay(messagesProcessedCount);
            await Task.Delay(waitDelay);
        }
    }

    private int GetNextWaitDelay(int lastMessagesProcessedCount)
    {
        if (lastMessagesProcessedCount == 0)
        {
            _noMessagesCount = Math.Max(7, ++_noMessagesCount);
        }
        else
        {
            _noMessagesCount = 0;
        }
        return 1000 * (2 ^ _noMessagesCount) + Random.Shared.Next(0, 1000); // 129 seconds is the limit
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");
        _executing = false;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _executing = false;
        GC.SuppressFinalize(this);
    }
}
