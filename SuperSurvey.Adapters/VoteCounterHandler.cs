
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using SuperSurvey.UseCases.Ports.In;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SuperSurvey.WebApp.HostedServices;
public class VoteCounterHandler
{
    private readonly AmazonSQSClient _client;
    private readonly CountVotesUseCase _countVotesUseCase;
    private readonly string _queueUrl;

    public VoteCounterHandler(AmazonSQSClient client,
        CountVotesUseCase countVotesUseCase,
        string queueUrl)
    {
        _client = client;
        _countVotesUseCase = countVotesUseCase;
        _queueUrl = queueUrl;
    }

    public async Task Execute()
    {
        var request = new ReceiveMessageRequest
        {
            MaxNumberOfMessages = 10,
            QueueUrl = _queueUrl,
            WaitTimeSeconds = 20
        };

        var response = await _client.ReceiveMessageAsync(request);
        var votes = response.Messages
            .Where(m => !string.IsNullOrEmpty(m.Body))
            .Select(m => ParseVote(m));
        await _countVotesUseCase.CountVotes(votes);

        var deleteBatch = new DeleteMessageBatchRequest 
        {
            QueueUrl = _queueUrl 
        };
        foreach (var message in response.Messages)
        {
            deleteBatch.Entries.Add(new DeleteMessageBatchRequestEntry(message.MessageId, message.ReceiptHandle));
        }
        await _client.DeleteMessageBatchAsync(deleteBatch);
    }

    private UncountedVote ParseVote(Message message)
    {
        return JsonSerializer.Deserialize<UncountedVote>(message.Body);
    }
}
