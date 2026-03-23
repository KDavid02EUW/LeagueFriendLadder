using Npgsql;
using System.Data;

namespace LeagueFriendLadder.Api.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("PostgreSql")
                                ?? throw new InvalidOperationException("Connection string not found.");
        }

        public IDbConnection CreateConnection()
            => new NpgsqlConnection(_connectionString);
    }
}