using Questao5.Core.Messages;

namespace Questao5.Application.Commands.Requests
{
    public class AdicionarTransacaoCommand : Command
    {
        public string TransacaoId { get; set; }
        public string Requisicao { get; set; }

        public AdicionarTransacaoCommand(string transacaoId, string requisicao)
        {
            TransacaoId = transacaoId;
            Requisicao = requisicao;
        }
    }
}
