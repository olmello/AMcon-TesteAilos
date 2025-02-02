using Questao5.Application.Commands.Requests;
using Questao5.Core.Communication.Mediator;
using Questao5.Core.Messages.CommonMessages.Notifications;
using Questao5.Domain;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Infrastructure.Database.QueryStore.Responses;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Services
{
    public class ContaService : IContaService
    {
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IContaRepository _contaRepository;

        public ContaService(IMediatorHandler mediatorHandler, DatabaseConfig databaseConfig, IContaRepository contaRepository)
        {
            _mediatorHandler = mediatorHandler;
            _contaRepository = contaRepository;
        }

        public async Task<string> EfetuarTransacaoAsync(EfetuarMovimentacaoFinanceiraCommand transacao)
        {
            var conta = await ObterContaValida(transacao.ContaId);

            if (conta == null) return string.Empty;

            if (await ExisteTransacaoAsync(transacao.RequisicaoId))
            {
                var resultado = await ObterResultadoTransacaoAsync(transacao.RequisicaoId);
                if (!string.IsNullOrEmpty(resultado)) return resultado;
            }
            else
            {
                await AdicionarTransacaoAsync(transacao.RequisicaoId);
            }

            return await ProcessarMovimentoAsync(conta, transacao);
        }

        private async Task<ContaCorrente> ObterContaValida(string contaId)
        {
            ContaCorrente conta = await _contaRepository.ObterContaPorId(contaId);

            if (conta == null)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification(nameof(EfetuarTransacaoAsync), EStatusRequisicao.INVALID_ACCOUNT.ToString()));
                return null;
            }

            if (!conta.Ativo)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification(nameof(EfetuarTransacaoAsync), EStatusRequisicao.INACTIVE_ACCOUNT.ToString()));
                return null;
            }

            return conta;
        }

        private async Task<bool> ExisteTransacaoAsync(string transacaoId)
        {
            return await _mediatorHandler.EnviarComando(new ExisteTransacaoCommand(transacaoId));
        }

        private async Task<string> ObterResultadoTransacaoAsync(string transacaoId)
        {
            return await _mediatorHandler.EnviarComando<ObterResultadoTransacaoCommand, string>(new ObterResultadoTransacaoCommand(transacaoId));
        }

        private async Task AdicionarTransacaoAsync(string transacaoId)
        {
            await _mediatorHandler.EnviarComando(new AdicionarTransacaoCommand(transacaoId, transacaoId.ToString()));
        }

        private async Task<string> ProcessarMovimentoAsync(ContaCorrente conta, EfetuarMovimentacaoFinanceiraCommand transacao)
        {
            var movimento = new Movimento(conta.IdContaCorrente, transacao.TipoMovimento, transacao.ValorTotal);

            bool sucesso = await _contaRepository.AdicionarAsync(movimento);

            if (sucesso)
            {
                await _mediatorHandler.EnviarComando(new AdicionarResultadoTransacaoCommand(transacao.RequisicaoId, movimento.IdMovimento.ToString()));
                return movimento.IdMovimento;
            }

            return string.Empty;
        }

        public async Task<ExtratoBancarioQuery> ObterExtratoPorId(string contaId)
        {
            return await _contaRepository.ObterExtratoPorContaId(contaId);
        }
    }
}