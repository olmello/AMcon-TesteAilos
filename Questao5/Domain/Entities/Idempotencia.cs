using Questao5.Core.DomainObjects;
using Questao5.Core.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Questao5.Domain.Entities
{
    public class Idempotencia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ChaveIdempotencia { get; private set; }

        [MaxLength(1000)]
        public string Requisicao { get; private set; }

        [MaxLength(1000)]
        public string Resultado { get; private set; }

        public Idempotencia(string chaveIdempotencia, string requisicao = null, string resultado = null)
        {
            ChaveIdempotencia = chaveIdempotencia;
            Requisicao = requisicao;
            Resultado = resultado;

            Validar();
        }

        private void Validar()
        {
            Validacoes.ValidarTamanho(ChaveIdempotencia, 33, 37, "Conta corrente inválida");

            if(!Requisicao.IsNullOrEmpty()) Validacoes.ValidarTamanho(Requisicao, 1, 1000, "Requisição inválida");
        }
    }
}