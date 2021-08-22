using Dapper;
using MySql.Data.MySqlClient;
using SuperSurvey.Domain;
using SuperSurvey.UseCases.Ports.Out;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public Task<List<Poll>> GetAllActive(DateTime currentDateTime)
        {
            throw new NotImplementedException();
        }

        public async Task<Poll> GetById(int id)
        {
            string sql = "SELECT * FROM Polls WHERE Id = @Id;";
            using (var connection = new MySqlConnection(_connectionString))
            {
                var poll = await connection.QuerySingleAsync<Poll>(sql, new { Id = id });
                return poll;
            }
        }

        public Task Save(Poll poll)
        {
            throw new NotImplementedException();
        }
    }
}
