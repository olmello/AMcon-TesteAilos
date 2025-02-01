using Questao5.Application.Commands.Requests;
using Questao5.Infrastructure.Database.QueryStore.Responses;

namespace Questao5.Infrastructure.Services
{
    public interface IContaService
    {
        Task<string> EfetuarTransacaoAsync(EfetuarMovimentacaoFinanceiraCommand transacao);
        Task<ExtratoBancarioQuery> ObterExtratoPorId(string contaId); 
    }
}