using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using SuperSurvey.Domain;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SuperSurvey.Adapters.Tests
{
    public class MySQLPollRepositoryTests
    {
        private readonly string CREATE_TABLE_SCRIPT =@"
            USE db;
            CREATE TABLE `Polls` (
	            `Id` INT NOT NULL,
	            `Name` VARCHAR(255) NOT NULL,
	            `ExpiresAt` DATETIME NOT NULL,
	            `UpdatedAt` DATETIME NOT NULL,
	            KEY `Ix_Expires` (`ExpiresAt`) USING BTREE,
                PRIMARY KEY(`Id`)
            );
            CREATE TABLE `Options` (
	            `Id` INT NOT NULL,
                `PollId` INT NOT NULL,
	            `Description` VARCHAR(255) NOT NULL,
                `PictureUrl` VARCHAR(255) NULL,
	            `VoteCount` INT NOT NULL,
	            `UpdatedAt` DATETIME NOT NULL,
                PRIMARY KEY(`Id`),
                FOREIGN KEY (PollId) REFERENCES Polls(Id)
            );
        ";

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Should_ReturnActivePolls_When_RepositoryIsQueriedByExpirationDate()
        {
            var testcontainersBuilder = new TestcontainersBuilder<MySqlTestcontainer>()
              .WithDatabase(new MySqlTestcontainerConfiguration
              {
                  Database = "db",
                  Username = "davidbowie",
                  Password = "secret",
              })
              ;

            await using var testcontainer = testcontainersBuilder.Build();
            await testcontainer.StartAsync();
            await testcontainer.ExecScriptAsync(CREATE_TABLE_SCRIPT);
            await testcontainer.ExecScriptAsync(@"
                INSERT INTO Polls (Id, Name, ExpiresAt, UpdatedAt) VALUES (1, 'ABC', '2021-08-21T14:56', '2021-08-23T09:03');
                INSERT INTO Polls (Id, Name, ExpiresAt, UpdatedAt) VALUES (2, 'ABC', '2021-08-23T14:56', '2021-08-23T09:03');
                INSERT INTO Polls (Id, Name, ExpiresAt, UpdatedAt) VALUES (3, 'ABC', '2021-08-25T14:56', '2021-08-23T09:03');"
            );

            var sut = new MySQLPollRepository(testcontainer.ConnectionString);
            var polls = await sut.GetAllActive(DateTime.Parse("2021-08-23T09:03"));

            polls.Count.Should().Be(2);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Should_ReturnExistingData_When_RepositoryIsQueriedById()
        {
            var testcontainersBuilder = new TestcontainersBuilder<MySqlTestcontainer>()
              .WithDatabase(new MySqlTestcontainerConfiguration
              {
                  Database = "db",
                  Username = "davidbowie",
                  Password = "secret",
              })
              ;

            await using var testcontainer = testcontainersBuilder.Build();
            await testcontainer.StartAsync();
            await testcontainer.ExecScriptAsync(CREATE_TABLE_SCRIPT);
            await testcontainer.ExecScriptAsync(
                "INSERT INTO Polls (Id, Name, ExpiresAt, UpdatedAt) VALUES (123, 'ABC', '2021-08-25T14:56', '2021-08-23T09:03');"
                );

            var sut = new MySQLPollRepository(testcontainer.ConnectionString);
            var poll = await sut.GetById(123);

            poll.Should().NotBeNull();
            poll.Id.Should().Be(123);
            poll.Name.Should().Be("ABC");
            poll.ExpiresAt.Should().Be(DateTime.Parse("2021-08-25T14:56"));
            poll.UpdatedAt.Should().Be(DateTime.Parse("2021-08-23T09:03"));
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Should_SavePollResults_When_DataIsUpdated()
        {
            var testcontainersBuilder = new TestcontainersBuilder<MySqlTestcontainer>()
              .WithDatabase(new MySqlTestcontainerConfiguration
              {
                  Database = "db",
                  Username = "davidbowie",
                  Password = "secret",
              })
              ;

            await using var testcontainer = testcontainersBuilder.Build();
            await testcontainer.StartAsync();
            await testcontainer.ExecScriptAsync(CREATE_TABLE_SCRIPT);
            await testcontainer.ExecScriptAsync(@"
                INSERT INTO Polls (Id, Name, ExpiresAt, UpdatedAt) VALUES (123, 'ABC', '2021-08-25T14:56', '2021-08-23T09:03');
                INSERT INTO Options (Id, PollId, Description, PictureUrl, VoteCount, UpdatedAt) VALUES (1, 123, 'ABC Option 1', NULL, 10, '2021-08-23T09:03');
                INSERT INTO Options (Id, PollId, Description, PictureUrl, VoteCount, UpdatedAt) VALUES (2, 123, 'ABC Option 2', 'http://someurl', 13, '2021-08-23T09:03');
                ");

            var poll = new Poll.Builder()
                .WithId(123)
                .WithName("New ABC")
                .WithExpiresAt(DateTime.Parse("2021-08-25T14:56"))
                .WithUpdatedAt(DateTime.Parse("2021-08-23T09:03"))
                .WithOption(new Option.Builder()
                    .WithId(2)
                    .WithDescription("New ABC Option 2")
                    .WithPictureUrl("http://newurl")
                    .WithVoteCount(20)
                    .WithUpdatedAt(DateTime.Parse("2021-08-23T09:03"))
                    .Build())
                .WithOption(new Option.Builder()
                    .WithId(3)
                    .WithDescription("New ABC Option 3")
                    .WithVoteCount(10)
                    .WithUpdatedAt(DateTime.Parse("2021-08-23T09:03"))
                    .Build())
                .Build();

            var sut = new MySQLPollRepository(testcontainer.ConnectionString);

            poll = await sut.Save(poll);

            poll.Should().NotBeNull();
            poll.Id.Should().Be(123);
            poll.Name.Should().Be("New ABC");
            poll.UpdatedAt.Should().NotBe(DateTime.Parse("2021-08-23T09:03"));
            poll.Options.Count.Should().Be(2);
            var option = poll.Options.Single(o => o.Id == 2);
            option.Description.Should().Be("New ABC Option 2");
            option.PictureUrl.Should().Be("http://newurl");
        }
    }
}