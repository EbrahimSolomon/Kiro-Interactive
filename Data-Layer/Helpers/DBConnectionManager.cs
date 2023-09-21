using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace Kiron_Interactive.Data_Layer.Helpers
{
    public class DBConnectionManager
    {
        private static readonly int MAX_CONNECTIONS = 10;
        private static readonly ConcurrentBag<IDbConnection> _connections = new ConcurrentBag<IDbConnection>();
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private static readonly ConnectionCreator _connectionCreator = new ConnectionCreator();

        public static IDbConnection GetOpenConnection(string connectionString)
        {
            _semaphore.Wait();

            try
            {
                // Try to get an existing open connection
                if (_connections.TryTake(out IDbConnection existingConnection) && existingConnection.State == ConnectionState.Open)
                {
                    return existingConnection;
                }

                // If no available open connection or they exceed the limit, create a new one
                if (_connections.Count < MAX_CONNECTIONS)
                {
                    var connection = _connectionCreator.CreateNewConnection(connectionString);
                    _connections.Add(connection);
                    return connection;
                }

                throw new Exception("Max number of connections exceeded.");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public static void CloseConnection(IDbConnection connection)
        {
            if (connection == null) return;

            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
                _connections.Add(connection);
            }
        }
    }
}