using FluentValidation;
using Questao5.Core.Messages;
using Questao5.Domain.Enumerators;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;


namespace Questao5.Application.Commands.Requests
{
    public class EfetuarMovimentacaoFinanceiraCommand : Command
    {
        /// <summary>
        /// Identificador único da transação, exemplo: 09003037-530B-4324-B9D2-4F6968728D14
        /// </summary>
        [Required]
        [MinLength(37)]
        public string RequisicaoId { get; set; }
        /// <summary>
        /// Identificador da conta associada à transação, exemplo: BB1A2541-57C2-455A-9A57-277A8A7EF2B5
        /// </summary>
        [Required]
        [MinLength(37)]
        public string ContaId { get; set; }
        /// <summary>
        /// Valor total da transação, exemplo: 298.75
        /// </summary>
        [Required]
        public double ValorTotal { get; set; }
        /// <summary>
        /// Tipo de movimento, exemplo: 'C' (Crédito), 'D'(Débito)
        /// </summary>
        [Required]
        [StringLength(1, MinimumLength = 1)]
        public string? TipoMovimento { get; set; }

        public override bool EhValido()
        {
            ValidationResult = new EfetuarMovimentacaoFinanceiraCommandValidation().Validate(this);

            return ValidationResult.IsValid;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class EfetuarMovimentacaoFinanceiraCommandValidation : AbstractValidator<EfetuarMovimentacaoFinanceiraCommand>
    {
        private readonly int _min_length = 30;
        private readonly int _max_length = 37;
        public EfetuarMovimentacaoFinanceiraCommandValidation()
        {
            RuleFor(c => c.RequisicaoId)
                .NotEmpty()
                .WithMessage(EStatusRequisicao.INVALID_TOKEN.ToString())
                .Length(_min_length, _max_length);

            RuleFor(c => c.ContaId)
                .NotEmpty()
                .WithMessage(EStatusRequisicao.INVALID_ACCOUNT.ToString())
                .Length(_min_length, _max_length);

            RuleFor(c => c.ValorTotal)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(EStatusRequisicao.INVALID_VALUE.ToString());

            RuleFor(c => c.TipoMovimento)
                .NotEmpty()
                .When(c => !string.IsNullOrEmpty(c.TipoMovimento))
                .Must(TransacaoValida)
                .WithMessage(EStatusRequisicao.INVALID_TYPE.ToString());
        }

        private bool TransacaoValida(string? movimento)
        {
            if (!Enum.TryParse(movimento, out ETipoMovimento type)) return false;

            return !type.Equals(ETipoMovimento.INVALID_TYPE);
        }
    }

    public class EfetuarMovimentacaoFinanceiraCommandExample : IExamplesProvider<EfetuarMovimentacaoFinanceiraCommand>
    {
        public EfetuarMovimentacaoFinanceiraCommand GetExamples()
        {
            return new EfetuarMovimentacaoFinanceiraCommand
            {
                RequisicaoId = Guid.NewGuid().ToString().ToUpper(),
                ContaId = Guid.NewGuid().ToString().ToUpper(),
                ValorTotal = 150.75,
                TipoMovimento = "C"
            };
        }
    }
}
