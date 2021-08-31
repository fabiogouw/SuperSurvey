
using Amazon.SQS;
using SuperSurvey.UseCases.Ports.In;

namespace SuperSurvey.WebApp.HostedServices;
public class VoteCounterHostedService : IHostedService, IDisposable
{
    private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
    private readonly SQSVoteCounterHandler _handler;
    private readonly ILogger<VoteCounterHostedService> _logger;
    private Timer _timer;

    public VoteCounterHostedService(ILogger<VoteCounterHostedService> logger,
        AmazonSQSClient client,
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
