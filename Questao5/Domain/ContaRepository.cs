using Microsoft.Data.Sqlite;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.QueryStore.Responses;
using Questao5.Infrastructure.Sqlite;
using System.Data;

namespace Questao5.Domain
{
    public class ContaRepository : IContaRepository
    {
        private readonly IDbConnection _connection;

        public ContaRepository(DatabaseConfig databaseConfig)
        {
            _connection = new SqliteConnection(databaseConfig.Name);
            _connection.Open();
        }

        public async Task<bool> AdicionarAsync(Movimento movimento)
        {
            string sql = @"INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) 
                                          VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";

            using var command = new SqliteCommand(sql, (SqliteConnection)_connection);

            command.Parameters.AddWithValue("@IdMovimento", movimento.IdMovimento);
            command.Parameters.AddWithValue("@IdContaCorrente", movimento.IdContaCorrente);
            command.Parameters.AddWithValue("@DataMovimento", movimento.DataMovimento);
            command.Parameters.AddWithValue("@TipoMovimento", movimento.TipoMovimento);
            command.Parameters.AddWithValue("@Valor", movimento.Valor);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<ExtratoBancarioQuery> ObterExtratoPorContaId(string accountId)
        {
            string sql = @"WITH MovimentoCalculado AS (
                             SELECT 
                                 idcontacorrente,
                                 COALESCE(SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE 0 END), 0) AS SomaCreditos,
                                 COALESCE(SUM(CASE WHEN tipomovimento = 'D' THEN valor ELSE 0 END), 0) AS SomaDebitos
                             FROM movimento
                             GROUP BY idcontacorrente
                         )
                         SELECT 
                             cc.numero AS NumeroConta,
                             cc.nome AS NomeTitular,
                             COALESCE(mc.SomaCreditos, 0) - COALESCE(mc.SomaDebitos, 0) AS SaldoAtual
                         FROM contacorrente cc
                         LEFT JOIN MovimentoCalculado mc ON cc.idcontacorrente = mc.idcontacorrente
                         WHERE cc.idcontacorrente = @NumeroConta";

            using var command = new SqliteCommand(sql, (SqliteConnection)_connection);

            command.Parameters.AddWithValue("@NumeroConta", accountId.ToUpper());

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ExtratoBancarioQuery
                {
                    NumeroConta = reader.GetInt32(0),
                    NomeTitular = reader.GetString(1),
                    DataExtrato = DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
                    SaldoAtual = reader.GetDouble(2)
                };
            }

            return null;
        }

        public async Task<ContaCorrente> ObterContaPorId(string accountId)
        {
            string sql = "SELECT idcontacorrente, ativo FROM contacorrente WHERE idcontacorrente = @NumeroConta";

            using var command = new SqliteCommand(sql, (SqliteConnection)_connection);

            command.Parameters.AddWithValue("@NumeroConta", accountId.ToUpper());

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ContaCorrente
                {
                    IdContaCorrente = reader.GetString(0),
                    Ativo = reader.GetBoolean(1),
                };
            }

            return null;
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}