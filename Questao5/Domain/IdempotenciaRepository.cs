using Microsoft.Data.Sqlite;
using Questao5.Core.Communication.Mediator;
using Questao5.Core.Messages.CommonMessages.Notifications;
using Questao5.Domain.Entities;
using Questao5.Setup;

namespace Questao5.Domain
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private DatabaseService _databaseService;
        private IMediatorHandler _mediatorHandler;

        public IdempotenciaRepository(DatabaseService databaseService, IMediatorHandler mediatorHandler)
        {
            _databaseService = databaseService;
            _databaseService.ConnectionOpen();
            _mediatorHandler = mediatorHandler;
        }

        public async Task<bool> AdicionarAsync(Idempotencia idempotencia)
        {
            string sql = "INSERT INTO idempotencia (chave_idempotencia, requisicao) VALUES (@ChaveIdempotencia, @Requisicao)";

            try
            {
                using var command = _databaseService.ObterCommand(sql);

                using var transaction = await _databaseService.ObterTransaction();

                command.Parameters.Add("@ChaveIdempotencia", SqliteType.Text).Value = idempotencia.ChaveIdempotencia.ToUpper();
                command.Parameters.Add("@Requisicao", SqliteType.Text).Value = idempotencia.Requisicao;

                return await _databaseService.ExecutarComandoTransacaoAsync(command, transaction);
            }
            catch (Exception exception)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("AdicionarAsync", exception.Message));

                return false;
            }
        }

        public async Task<bool> Existe(string transactionId)
        {
            string sql = "SELECT COUNT(1) FROM idempotencia WHERE chave_idempotencia = @ChaveIdempotencia";

            try
            {
                using var command = _databaseService.ObterCommand(sql);

                command.Parameters.Add("@ChaveIdempotencia", SqliteType.Text).Value = transactionId.ToUpper();

                return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
            }
            catch (Exception exception)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("AdicionarAsync", exception.Message));

                return false;
            }
        }

        public async Task<bool> AtualizarAsync(Idempotencia idempotencia)
        {
            string sql = @"UPDATE idempotencia 
                           SET resultado = @Resultado
                           WHERE chave_idempotencia = @ChaveIdempotencia";

            try
            {
                using var command = _databaseService.ObterCommand(sql);

                using var transaction = await _databaseService.ObterTransaction();

                command.Parameters.Add("@ChaveIdempotencia", SqliteType.Text).Value = idempotencia.ChaveIdempotencia.ToUpper();
                command.Parameters.Add("@Resultado", SqliteType.Text).Value = idempotencia.Resultado.ToUpper();

                return await _databaseService.ExecutarComandoTransacaoAsync(command, transaction);
            }
            catch (Exception exception)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("AdicionarAsync", exception.Message));

                return false;
            }
        }

        public async Task<string> ObterTransacaoPorIdAsync(string transactionId)
        {
            string sql = "SELECT resultado FROM idempotencia WHERE chave_idempotencia = @TransactionId";

            try
            {
                using var command = _databaseService.ObterCommand(sql);

                command.Parameters.Add("@TransactionId", SqliteType.Text).Value = transactionId?.ToUpper() ?? string.Empty;

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return reader["resultado"].ToString();
                }

                return string.Empty;
            }
            catch (Exception exception)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("ObterContaPorId", exception.Message));

                return string.Empty;
            }
        }

        public void Dispose()
        {
            _databaseService.Dispose();
        }
    }
}