using Questao5.Core.Messages;

namespace Questao5.Application.Commands.Requests
{
    public class ExisteTransacaoCommand : Command 
    {
        public string TransacaoId { get; set; }

        public ExisteTransacaoCommand(string transacaoId)
        {
            TransacaoId = transacaoId;
        }
    }
}
