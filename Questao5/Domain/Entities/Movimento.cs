using Questao5.Core.DomainObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Questao5.Domain.Entities
{
    public class Movimento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string IdMovimento { get; private set; }

        [Required]
        public string IdContaCorrente { get; private set; }

        [ForeignKey("IdContaCorrente")]
        public ContaCorrente ContaCorrente { get; private set; }

        [Required]
        public DateTime DataMovimento { get; private set; }

        [Required]
        [MaxLength(1)]
        public string TipoMovimento { get; private set; }

        [Required]
        [Column(TypeName = "REAL(18,2)")]
        public double Valor { get; private set; }

        public Movimento(string ContaCorrenteId, string tipoMovimento, double valor)
        {
            IdMovimento = Guid.NewGuid().ToString().ToUpper();
            DataMovimento = DateTime.Now;
            IdContaCorrente = ContaCorrenteId.ToUpper();
            TipoMovimento = tipoMovimento.ToUpper();
            Valor = valor;

            Validar();
        }

        private void Validar()
        {
            Validacoes.ValidarSeMenorQue(Valor, 0.1D, "O valor precisa ser no mínimo 1");
            Validacoes.ValidarSeDiferente("^[CD]{1}$", TipoMovimento, "O TipoMovimento deve ser 'C' ou 'D'");
            Validacoes.ValidarTamanho(IdContaCorrente, 33, 37, "Conta corrente inválida");
        }

        public override string ToString()
        {
            return IdMovimento;
        }
    }
}