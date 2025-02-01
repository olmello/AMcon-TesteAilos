using Microsoft.Data.Sqlite;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;
using System.Data;

namespace Questao5.Domain
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private IDbConnection _connection;

        public IdempotenciaRepository(DatabaseConfig databaseConfig)
        {
            _connection = new SqliteConnection(databaseConfig.Name);
            _connection.Open();
        }

        public async Task<bool> AdicionarAsync(Idempotencia idempotencia)
        {
            string sql = "INSERT INTO idempotencia (chave_idempotencia, requisicao) VALUES (@ChaveIdempotencia, @Requisicao)";

            using var command = new SqliteCommand(sql, (SqliteConnection)_connection);

            command.Parameters.AddWithValue("@ChaveIdempotencia", idempotencia.ChaveIdempotencia.ToUpper());
            command.Parameters.AddWithValue("@Requisicao", idempotencia.Requisicao);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> Existe(string transactionId)
        {
            string sql = "SELECT COUNT(1) FROM idempotencia WHERE chave_idempotencia = @ChaveIdempotencia";

            using var command = new SqliteCommand(sql, (SqliteConnection)_connection);

            command.Parameters.AddWithValue("@ChaveIdempotencia", transactionId.ToUpper());

            return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
        }

        public async Task<bool> AtualizarAsync(Idempotencia idempotencia)
        {
            string sql = @"UPDATE idempotencia 
                           SET resultado = @Resultado
                           WHERE chave_idempotencia = @ChaveIdempotencia";

            using var command = new SqliteCommand(sql, (SqliteConnection)_connection);

            command.Parameters.AddWithValue("@ChaveIdempotencia", idempotencia.ChaveIdempotencia.ToUpper());
            command.Parameters.AddWithValue("@Resultado", idempotencia.Resultado.ToUpper());

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<string> ObterTransacaoPorIdAsync(string transactionId)
        {
            string sql = "SELECT resultado FROM idempotencia WHERE chave_idempotencia = @TransactionId";

            using var command = new SqliteCommand(sql, (SqliteConnection)_connection);

            command.Parameters.AddWithValue("@TransactionId", transactionId.ToUpper());

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader["resultado"].ToString();
            }

            return string.Empty;
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}