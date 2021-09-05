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
        private readonly IAmazonSQS _client;
        private readonly string _queueUrl;

        public SQSVoteRepository(IAmazonSQS client,
            string queueUrl)
        {
            _client = client;
            _queueUrl = queueUrl;
        }

        public async Task Save(VoteCommand command)
        {
            string messageBody = JsonSerializer.Serialize(command);
            await _client.SendMessageAsync(_queueUrl, messageBody);
        }
    }
}
