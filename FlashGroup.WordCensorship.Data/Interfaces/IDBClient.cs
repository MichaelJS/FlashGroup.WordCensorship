using Microsoft.Data.SqlClient;
using System.Data;

namespace FlashGroup.WordCensorship.Data
{
    public interface IDBClient
    {
        Task<SqlDataReader> GetReaderAsync(string getScript);
        Task<int> Execute(string executeScript, params SqlParameter[] scriptParams);
    }
}
