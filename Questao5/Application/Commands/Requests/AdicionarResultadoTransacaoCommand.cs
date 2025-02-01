using Questao5.Core.Messages;

namespace Questao5.Application.Commands.Requests
{
    public class AdicionarResultadoTransacaoCommand : Command
    {
        public string TransacaoId { get; set; }
        public string Resultado { get; set; }

        public AdicionarResultadoTransacaoCommand(string transacaoId, string resultado)
        {
            TransacaoId = transacaoId;
            Resultado = resultado;
        }
    }
}
