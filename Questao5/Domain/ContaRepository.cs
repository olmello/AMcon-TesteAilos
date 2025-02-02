using Microsoft.Data.Sqlite;
using Questao5.Core.Communication.Mediator;
using Questao5.Core.Messages.CommonMessages.Notifications;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.QueryStore.Responses;
using Questao5.Setup;

namespace Questao5.Domain
{
    public class ContaRepository : IContaRepository
    {
        private IMediatorHandler _mediatorHandler;
        private DatabaseService _databaseService;

        public ContaRepository(DatabaseService databaseService, IMediatorHandler mediatorHandler)
        {
            _databaseService = databaseService;
            _databaseService.ConnectionOpen();
            _mediatorHandler = mediatorHandler;
        }

        public async Task<bool> AdicionarAsync(Movimento movimento)
        {
            try
            {
                string sql = @"INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) 
                                          VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";

                using var command = _databaseService.ObterCommand(sql);

                command.Parameters.Add("@IdMovimento", SqliteType.Text).Value = movimento.IdMovimento;
                command.Parameters.Add("@IdContaCorrente", SqliteType.Integer).Value = movimento.IdContaCorrente;
                command.Parameters.Add("@DataMovimento", SqliteType.Text).Value = movimento.DataMovimento;
                command.Parameters.Add("@TipoMovimento", SqliteType.Text).Value = movimento.TipoMovimento;
                command.Parameters.Add("@Valor", SqliteType.Real).Value = movimento.Valor;

                using var transaction = await _databaseService.ObterTransaction();

                return await _databaseService.ExecutarComandoTransacaoAsync(command, transaction);
            }
            catch (Exception exception)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("AdicionarAsync", exception.Message));

                return false;
            }
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

            try
            {
                using var command = _databaseService.ObterCommand(sql);

                command.Parameters.Add("@NumeroConta", SqliteType.Text).Value = accountId.ToUpper();

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
            catch (Exception exception)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("ObterExtratoPorContaId", exception.Message));
                return null;
            }
        }

        public async Task<ContaCorrente> ObterContaPorId(string accountId)
        {
            string sql = "SELECT idcontacorrente, ativo FROM contacorrente WHERE idcontacorrente = @NumeroConta";

            try
            {
                using var command = _databaseService.ObterCommand(sql);

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
            catch (Exception exception)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("ObterContaPorId", exception.Message));
                return null;
            }
        }

        public void Dispose()
        {
            _databaseService.Dispose();
        }
    }
}