using Amazon.SQS;
using Amazon.SQS.Model;
using SuperSurvey.UseCases.Ports.In;
using System.Text.Json;

namespace SuperSurvey.WebApp.HostedServices;
public class SQSVoteCounterHandler
{
    private readonly IAmazonSQS _client;
    private readonly CountVotesUseCase _countVotesUseCase;
    private readonly string _queueUrl;

    public SQSVoteCounterHandler(IAmazonSQS client,
        CountVotesUseCase countVotesUseCase,
        string queueUrl)
    {
        _client = client;
        _countVotesUseCase = countVotesUseCase;
        _queueUrl = queueUrl;
    }

    public async Task<int> Execute()
    {
        var request = new ReceiveMessageRequest
        {
            MaxNumberOfMessages = 10,
            QueueUrl = _queueUrl,
            WaitTimeSeconds = 20
        };

        var response = await _client.ReceiveMessageAsync(request);
        var voteCommands = response.Messages
            .Where(message => !string.IsNullOrEmpty(message.Body))
            .Select(message => ParseVote(message));

        await _countVotesUseCase.CountVotes(voteCommands);

        var deleteBatch = new DeleteMessageBatchRequest 
        {
            QueueUrl = _queueUrl 
        };
        foreach (var message in response.Messages)
        {
            deleteBatch.Entries.Add(new DeleteMessageBatchRequestEntry(message.MessageId, message.ReceiptHandle));
        }
        await _client.DeleteMessageBatchAsync(deleteBatch);
        return response.Messages.Count;
    }

    private VoteCommand ParseVote(Message message)
    {
        return JsonSerializer.Deserialize<VoteCommand>(message.Body);
    }
}
