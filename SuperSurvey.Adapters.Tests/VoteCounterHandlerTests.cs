using Amazon.SQS;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SuperSurvey.UseCases.Ports.In;
using SuperSurvey.WebApp.HostedServices;

namespace SuperSurvey.Adapters.Tests
{
    [TestClass]
    public class VoteCounterHandlerTests
    {
        [TestMethod]
        [TestCategory("Integration")]
        public async Task Should_ReadMessageFromQueue_When_MessageIsAvailable()
        {
            var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                .WithImage("localstack/localstack:latest")
                .WithEnvironment("SERVICES", "sqs")
                .WithEnvironment("EDGE_PORT", "4566")
                .WithPortBinding(4566, 4566)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(4566))
                ;

            await using (var testcontainer = testcontainersBuilder.Build())
            {
                string queueName = "my-queue";
                await testcontainer.StartAsync();
                var client = new AmazonSQSClient("test", "test", new AmazonSQSConfig()
                {
                    ServiceURL = $"http://localhost:4566"
                });
                await client.CreateQueueAsync(queueName);
                string queueUrl = (await client.GetQueueUrlAsync(queueName)).QueueUrl;
                await client.SendMessageAsync(queueUrl, "test");

                var useCaseMock = new Mock<CountVotesUseCase>();

                var sut = new VoteCounterHandler(client, useCaseMock.Object, queueUrl);

                await sut.Execute();
                
                var currentMessages = await client.ReceiveMessageAsync(queueUrl);
                currentMessages.Messages.Count.Should().Be(0);
            }
        }
    }
}
