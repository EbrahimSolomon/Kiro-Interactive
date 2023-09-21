using System.Data;
using System.Data.SqlClient;

namespace Kiron_Interactive.Data_Layer.Helpers
{
    public class ConnectionCreator
    {
        public IDbConnection CreateNewConnection(string connectionString)
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
