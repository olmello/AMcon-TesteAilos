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
            IdContaCorrente = ContaCorrenteId.ToUpper();
            DataMovimento = DateTime.Now;
            TipoMovimento = tipoMovimento.ToUpper();
            Valor = valor;
        }

        public override string ToString()
        {
            return IdMovimento.ToString();
        }
    }
}