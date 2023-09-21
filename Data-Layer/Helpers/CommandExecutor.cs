using System.Data;
using Dapper;

namespace Kiron_Interactive.Data_Layer.Helpers
{
    public class CommandExecutor : IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public CommandExecutor(string connectionString)
        {
            _connection = DBConnectionManager.GetOpenConnection(connectionString);
        }

        public void BeginTransaction()
        {
            try
            {
                _transaction = _connection.BeginTransaction();
            }
            catch (Exception ex)
            {
                // Handle the exception, perhaps logging or re-throwing with more info
                throw new Exception("Error starting a database transaction.", ex);
            }
        }

        public void Commit()
        {
            try
            {
                _transaction?.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while committing the transaction.", ex);
            }
            finally
            {
                DBConnectionManager.CloseConnection(_connection);
            }
        }

        public void Rollback()
        {
            try
            {
                _transaction?.Rollback();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while rolling back the transaction.", ex);
            }
            finally
            {
                DBConnectionManager.CloseConnection(_connection);
            }
        }

         public async Task<T> ExecuteStoredProcedureAsync<T>(string storedProcedureName, object parameters = null)
    {
        try
        {
            return await _connection.QueryFirstOrDefaultAsync<T>(
                storedProcedureName,
                parameters,
                _transaction,
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing the stored procedure {storedProcedureName}.", ex);
        }
    }

      public async Task<IEnumerable<T>> ExecuteStoredProcedureGetListAsync<T>(string storedProcedureName, object parameters = null)
    {
        try
    {
        return await _connection.QueryAsync<T>(
            storedProcedureName,
            parameters,
            _transaction,
            commandType: CommandType.StoredProcedure);
    }
    catch (Exception ex)
    {
        throw new Exception($"Error executing the stored procedure {storedProcedureName}.", ex);
    }
    }
    
        public void Dispose()
        {
            _transaction?.Dispose();
            DBConnectionManager.CloseConnection(_connection);
            _connection = null;
        }
    }
}