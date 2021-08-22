
using Amazon.SQS;

namespace SuperSurvey.WebApp.HostedServices;
public class VoteCounterHostedService : IHostedService, IDisposable
{
    private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
    private readonly AmazonSQSClient client = new AmazonSQSClient();
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
