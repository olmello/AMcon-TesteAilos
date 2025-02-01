using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.QueryStore.Responses;

namespace Questao5.Domain
{
    public interface IContaRepository : IDisposable
    {
        Task<bool> AdicionarAsync(Movimento movimento);
        Task<ExtratoBancarioQuery> ObterExtratoPorContaId(string accountId);
        Task<ContaCorrente> ObterContaPorId(string accountId);
    }
}