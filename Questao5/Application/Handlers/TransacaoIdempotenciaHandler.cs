using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Domain;
using Questao5.Domain.Entities;

namespace Questao5.Application.Handlers
{
    public class TransacaoIdempotenciaHandler : IRequestHandler<AdicionarTransacaoCommand, bool>,
                                                IRequestHandler<AdicionarResultadoTransacaoCommand, bool>,
                                                IRequestHandler<ExisteTransacaoCommand, bool>,
                                                IRequestHandler<ObterResultadoTransacaoCommand, string>
        
    {
        private readonly IIdempotenciaRepository _idempotenciaRepository;

        public TransacaoIdempotenciaHandler(IIdempotenciaRepository idempotenciaRepository)
        {
            _idempotenciaRepository = idempotenciaRepository;
        }

        public async Task<bool> Handle(AdicionarTransacaoCommand command, CancellationToken cancellationToken)
        {
            return await _idempotenciaRepository.AdicionarAsync(new Idempotencia(command.TransacaoId, command.Requisicao));
        }

        public async Task<bool> Handle(AdicionarResultadoTransacaoCommand command, CancellationToken cancellationToken)
        {
            return await _idempotenciaRepository.AtualizarAsync(new Idempotencia(command.TransacaoId, ".", resultado: command.Resultado));
        }

        public async Task<bool> Handle(ExisteTransacaoCommand command, CancellationToken cancellationToken)
        {
            return await _idempotenciaRepository.Existe(command.TransacaoId);
        }

        public async Task<string> Handle(ObterResultadoTransacaoCommand command, CancellationToken cancellationToken)
        {
            return await _idempotenciaRepository.ObterTransacaoPorIdAsync(command.TransacaoId);
        }
    }
}