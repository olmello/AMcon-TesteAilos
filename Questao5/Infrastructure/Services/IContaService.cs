using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Application.Commands.Requests;
using Questao5.Core.Communication.Mediator;
using Questao5.Core.Messages.CommonMessages.Notifications;
using Questao5.Domain;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.QueryStore.Responses;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Services
{
    public interface IContaService
    {
        Task<string> EfetuarTransacaoAsync(TransacaoCommand transacao);
        Task<ExtratoBancarioQuery> ObterExtratoPorId(string contaId); 
    }

    public class ContaService : IContaService
    {
        private readonly IMediatorHandler _mediatorHandler;

        private readonly IContaRepository _contaRepository;

        public ContaService(IMediatorHandler mediatorHandler, DatabaseConfig databaseConfig, IContaRepository contaRepository)
        {
            _mediatorHandler = mediatorHandler;
            _contaRepository = contaRepository;
        }

        public async Task<string> EfetuarTransacaoAsync(TransacaoCommand transacao)
        {
            ContaCorrente conta = await _contaRepository.ObterContaPorId(transacao.ContaId);

            if (conta == null)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification(nameof(EfetuarTransacaoAsync), "Conta corrente inválida e/ou não cadastrada"));

                return string.Empty;
            }
            else if (!conta.Ativo)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification(nameof(EfetuarTransacaoAsync), "Conta corrente inválida e/ou inativa"));

                return string.Empty;
            }

            bool existe = await _mediatorHandler.EnviarComando(new ExisteTransacaoCommand(transacao.TransacaoId));

            if (existe)
            {
                string resultado = await _mediatorHandler.EnviarComando<ObterResultadoTransacaoCommand, string>(new ObterResultadoTransacaoCommand(transacao.TransacaoId));

                if (!string.IsNullOrEmpty(resultado)) return resultado;
            }
            else
            {
                await _mediatorHandler.EnviarComando(new AdicionarTransacaoCommand(transacao.TransacaoId, transacao.ToString()));
            }

            Movimento movimento = new (conta.IdContaCorrente, transacao.TipoMovimento, transacao.ValorTotal);

            bool sucesso = await _contaRepository.AdicionarAsync(movimento);
            
            if (sucesso) await _mediatorHandler.EnviarComando(new AdicionarResultadoTransacaoCommand(transacao.TransacaoId, movimento.IdMovimento.ToString()));

            return movimento.IdMovimento;
        }

        public async Task<ExtratoBancarioQuery> ObterExtratoPorId(string contaId)
        {
            return await _contaRepository.ObterExtratoPorContaId(contaId);
        }
    }
}