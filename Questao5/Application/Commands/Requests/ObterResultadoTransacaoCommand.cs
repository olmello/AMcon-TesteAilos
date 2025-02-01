using MediatR;
using Questao5.Core.Messages;

namespace Questao5.Application.Commands.Requests
{
    public class ObterResultadoTransacaoCommand : Command, IRequest<string>
    {
        public string TransacaoId { get; set; }

        public ObterResultadoTransacaoCommand(string transacaoId)
        {
            TransacaoId = transacaoId;
        }
    }
}
