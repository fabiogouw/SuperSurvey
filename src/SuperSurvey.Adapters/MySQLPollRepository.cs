using Dapper;
using MoreLinq;
using MySql.Data.MySqlClient;
using SuperSurvey.Domain;
using SuperSurvey.UseCases.Ports.Out;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperSurvey.Adapters
{
    public class MySQLPollRepository : PollRepository
    {
        private readonly string _connectionString;
        public MySQLPollRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<List<Poll>> GetAllActive(DateTime currentDateTime)
        {
            using var connection = new MySqlConnection(_connectionString);
            var polls = await connection.QueryAsync<Poll>("SELECT * FROM Polls WHERE ExpiresAt >= @ExpiresAt ORDER BY Name;", new { ExpiresAt = currentDateTime });
            polls = await GetOptionsByPollIds(connection, polls);
            return polls.ToList();
        }

        public async Task<Poll> GetById(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var poll = await connection.QuerySingleAsync<Poll>("SELECT * FROM Polls WHERE Id = @Id;", new { Id = id });
            poll = (await GetOptionsByPollIds(connection, new[] { poll })).Single();
            return poll;
        }

        private static async Task<IEnumerable<Poll>> GetOptionsByPollIds(MySqlConnection connection, IEnumerable<Poll> polls)
        {
            int[] pollIds = polls.Select(p => p.Id).ToArray();
            var options = await connection.QueryAsync("SELECT * FROM Options WHERE PollId IN @PollIds;", new { PollIds = pollIds });
            return polls.GroupJoin(options, poll => poll.Id, option => option.PollId, (poll, options) =>  
            {
                foreach(var option in options)
                {
                    poll.Options.Add(new Option.Builder()
                        .WithId(option.Id)
                        .WithPoll(poll)
                        .WithDescription(option.Description)
                        .WithPictureUrl(option.PictureUrl)
                        .WithVoteCount(option.VoteCount)
                        .WithUpdatedAt(option.UpdatedAt)
                        .Build());
                }
                return poll;
            });
        }

        public async Task<Poll> Save(Poll poll)
        {
            var currentPoll = await GetById(poll.Id);
            string sql = UpdateOrInsertPoll(currentPoll);
            var updatedTime = DateTime.Now;
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                await connection.ExecuteAsync(sql, new
                {
                    poll.Id,
                    poll.Name,
                    poll.ExpiresAt,
                    poll.UpdatedAt,
                    NewUpdatedAt = updatedTime
                }, transaction: transaction);

                var comparedOptions = currentPoll.Options.FullJoin(poll.Options, x => x.Id,
                    @left => GetDeleteOptionCommand(@left),
                    @right => GetInsertOptionCommand(@right, updatedTime),
                    (@left, @right) => GetUpdateOptionCommand(@left, @right, updatedTime));

                foreach (var comparison in comparedOptions)
                {
                    await connection.ExecuteAsync(comparison.Item1, comparison.Item2, transaction: transaction);
                }

                transaction.Commit();

            }
            return await GetById(poll.Id);
        }

        private static string UpdateOrInsertPoll(Poll currentPoll)
        {
            if(currentPoll == null)
            {
                return @"INSERT INTO Polls (Id, Name, ExpiresAt, UpdatedAt)
                            VALUES (@Id, @Name, @ExpiresAt, @UpdatedAt);
                        ";
            }
            return @"UPDATE Polls
                        SET Name = @Name,
                        ExpiresAt = @ExpiresAt,
                        UpdatedAt = @NewUpdatedAt
                    WHERE
                        Id = @Id 
                        AND UpdatedAt = @UpdatedAt;
                ";
        }

        private static (string, object) GetUpdateOptionCommand(Option current, Option updated, DateTime newUpdatedAt)
        {
            return ("UPDATE Options SET Description = @Description, PictureUrl = @PictureUrl, VoteCount = @VoteCount, UpdatedAt = @NewUpdatedAt WHERE Id = @Id AND UpdatedAt = @UpdatedAt;",
                new
                {
                    current.Id,
                    updated.Description,
                    updated.PictureUrl,
                    updated.VoteCount,
                    current.UpdatedAt,
                    NewUpdatedAt = newUpdatedAt
                });
        }

        private static (string, object) GetDeleteOptionCommand(Option current)
        {
            return ("DELETE FROM Options WHERE Id = @Id AND UpdatedAt = @UpdatedAt;",
                new
                {
                    current.Id,
                    current.UpdatedAt
                });
        }

        private static (string, object) GetInsertOptionCommand(Option updated, DateTime newUpdatedAt)
        {
            return ("INSERT INTO Options (Id, PollId, Description, PictureUrl, VoteCount, UpdatedAt) VALUES (@Id, @PollId, @Description, @PictureUrl, @VoteCount, @NewUpdatedAt);",
            new
            {
                updated.Id,
                PollId = updated.Poll.Id,
                updated.Description,
                updated.PictureUrl,
                updated.VoteCount,
                updated.UpdatedAt,
                NewUpdatedAt = newUpdatedAt
            });
        }
    }
}
