using Microsoft.Data.Sqlite;
using Questao5.Infrastructure.Sqlite;
using System.Data;
using System.Data.Common;

namespace Questao5.Setup
{
    public class DatabaseService : IDisposable
    {
        private readonly IDbConnection _connection;

        public DatabaseService(DatabaseConfig databaseConfig)
        {
            _connection = new SqliteConnection(databaseConfig.Name);
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        public void ConnectionOpen() => _connection.Open();

        public SqliteCommand ObterCommand(string sql) => new(sql, (SqliteConnection)_connection);

        public async Task<SqliteTransaction> ObterTransaction() => ((SqliteConnection)_connection).BeginTransaction();

        public async Task<bool> ExecutarComandoTransacaoAsync(SqliteCommand command, SqliteTransaction transaction)
        {
            try
            {
                command.Transaction = transaction;

                bool sucesso = await command.ExecuteNonQueryAsync() > 0;

                if (sucesso) await transaction.CommitAsync();

                else await transaction.RollbackAsync();

                return sucesso;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
