using Questao5.Domain.Entities;

namespace Questao5.Domain
{
    public interface IIdempotenciaRepository : IDisposable
    {
        Task<bool> AdicionarAsync(Idempotencia idempotencia);
        Task<bool> Existe(string transactionId);
        Task<bool> AtualizarAsync(Idempotencia idempotencia);
        Task<string> ObterTransacaoPorIdAsync(string transactionId);
    }
}