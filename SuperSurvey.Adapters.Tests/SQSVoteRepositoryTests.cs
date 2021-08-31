using Amazon.SQS;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperSurvey.UseCases.Ports.In;
using System.Text.Json;

namespace SuperSurvey.Adapters.Tests
{
    [TestClass]
    public class SQSVoteRepositoryTests
    {
        [TestMethod]
        [TestCategory("Integration")]
        public async Task Should_PostMessage_When_VoteCommandIsSaved()
        {
            int randomPort = Random.Shared.Next(49152, 65535);
            var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                .WithImage("localstack/localstack:latest")
                .WithEnvironment("SERVICES", "sqs")
                .WithEnvironment("EDGE_PORT", "4566")
                .WithPortBinding(randomPort, 4566)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(4566))
                ;

            await using (var testcontainer = testcontainersBuilder.Build())
            {
                string queueName = "my-queue";
                await testcontainer.StartAsync();
                var client = new AmazonSQSClient("test", "test", new AmazonSQSConfig()
                {
                    ServiceURL = $"http://localhost:{ randomPort }"
                });
                var queue = await client.CreateQueueAsync(queueName);
                var sut = new SQSVoteRepository(client, queue.QueueUrl);

                var voteCommand = new VoteCommand()
                {
                    PollId = 1,
                    SelectedOption = 999,
                    UserId = 2,
                    CreatedAt = DateTime.Now
                };
                await sut.Save(voteCommand);

                string queueUrl = (await client.GetQueueUrlAsync(queueName)).QueueUrl;
                var result = await client.ReceiveMessageAsync(queueUrl);
                result.Messages.Count.Should().Be(1);
                var message = result.Messages[0];
                message.Body.Should().Be(JsonSerializer.Serialize(voteCommand));
            }
        }
    }
}
