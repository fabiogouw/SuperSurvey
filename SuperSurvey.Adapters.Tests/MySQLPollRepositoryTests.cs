using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace SuperSurvey.Adapters.Tests;
[TestClass]
public class MySQLPollRepositoryTests
{
    private readonly string CREATE_TABLE_SCRIPT = 
        @"USE db;
        CREATE TABLE `Polls` (
	        `Id` INT NOT NULL,
	        `Name` VARCHAR(255) NOT NULL,
	        `Expires` DATETIME NOT NULL,
	        `UpdatedAt` DATETIME NOT NULL,
	        KEY `Ix_Expires` (`Expires`) USING BTREE,
            PRIMARY KEY(`Id`)
        );";

    [TestMethod]
    [TestCategory("Integration")]
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

        await using (var testcontainer = testcontainersBuilder.Build())
        {
            await testcontainer.StartAsync();
            await testcontainer.CopyFileAsync("structure.sql", Encoding.ASCII.GetBytes(CREATE_TABLE_SCRIPT));
            await testcontainer.CopyFileAsync("data.sql",
                Encoding.ASCII.GetBytes("INSERT INTO Polls (Id, Name, Expires, UpdatedAt) VALUES (123, 'Teste ABC', '1981-06-08', '1981-06-08');"));
            await testcontainer.ExecAsync(new []{ "/bin/sh", "-c", "mysql -udavidbowie -psecret db < /structure.sql", "" });
            await testcontainer.ExecAsync(new []{ "/bin/sh", "-c", "mysql -udavidbowie -psecret db < /data.sql", "" });

            var sut = new MySQLPollRepository(testcontainer.ConnectionString);
            var poll = await sut.GetById(123);

            poll.Should().NotBeNull();
            poll.Id.Should().Be(123);
        }
    }
}