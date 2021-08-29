using Amazon.SQS;
using Amazon.SQS.Model;
using SuperSurvey.Domain;
using SuperSurvey.UseCases.Ports.In;
using SuperSurvey.UseCases.Ports.Out;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SuperSurvey.Adapters
{
    public class SQSVoteRepository : VoteCommandRepository
    {
        private readonly AmazonSQSClient _client;
        private readonly string _queueName;
        private string _queueUrl;

        public SQSVoteRepository(AmazonSQSClient client,
            string queueName)
        {
            _client = client;
            _queueName = queueName;
        }

        public async Task Save(VoteCommand command)
        {
            if (string.IsNullOrEmpty(_queueUrl))
            {
                _queueUrl = (await _client.GetQueueUrlAsync(_queueName)).QueueUrl;
            }
            string messageBody = JsonSerializer.Serialize(command);
            await _client.SendMessageAsync(_queueUrl, messageBody);
        }
    }
}
