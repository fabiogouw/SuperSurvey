﻿using Amazon.SQS;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Xunit;
using Moq;
using SuperSurvey.UseCases.Ports.In;
using System.Threading.Tasks;
using System;

namespace SuperSurvey.Adapters.Tests
{
    public class SQSVoteCounterHandlerTests
    {
        [Fact]
        [Trait("Category", "Integration")]
        public async Task Should_ReadMessageFromQueue_When_MessageIsAvailable()
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
                var client = new AmazonSQSClient("anything", "anything", new AmazonSQSConfig()
                {
                    ServiceURL = $"http://localhost:{ randomPort }"
                });
                var queue = await client.CreateQueueAsync(queueName);

                await client.SendMessageAsync(queue.QueueUrl, "test");

                var useCaseMock = new Mock<CountVotesUseCase>();
                
                var sut = new SQSVoteCounterHandler(client, useCaseMock.Object, queue.QueueUrl);

                await sut.Execute();
                
                var result = await client.ReceiveMessageAsync(queue.QueueUrl);
                result.Messages.Count.Should().Be(0);
            }
        }
    }
}
