using Microsoft.Data.SqlClient;
using System.Data;

namespace FlashGroup.WordCensorship.Data
{
    public class DBClient : IDBClient
    {
        protected readonly string _connectionString;
        public DBClient(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<SqlDataReader> GetReaderAsync(string getScript)
        {
            var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var command = new SqlCommand(getScript, conn)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 5
            };

            return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        }

        public async Task<int> Execute(string executeScript, params SqlParameter[] scriptParams)
        {
            await using var conn = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(executeScript, conn)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 5
            };

            foreach (var scriptParam in scriptParams)
            {
                command.Parameters.AddWithValue("@" + scriptParam.ParameterName, scriptParam.Value);
            }

            await conn.OpenAsync();

            return await command.ExecuteNonQueryAsync();
        }
    }
}
