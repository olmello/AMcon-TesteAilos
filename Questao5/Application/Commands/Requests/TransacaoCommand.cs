using FluentValidation;
using Questao5.Core.Messages;
using Questao5.Domain.Enumerators;
using System.Text.Json;
using Swashbuckle.AspNetCore.Filters;


namespace Questao5.Application.Commands.Requests
{
    public class TransacaoCommand : Command
    {
        /// <summary>
        /// Identificador único da transação, exemplo: 09003037-530B-4324-B9D2-4F6968728D14
        /// </summary>
        public string TransacaoId { get; set; }
        /// <summary>
        /// Identificador da conta associada à transação, exemplo: BB1A2541-57C2-455A-9A57-277A8A7EF2B5
        /// </summary>
        public string ContaId { get; set; }
        /// <summary>
        /// Valor total da transação, exemplo: 298.75
        /// </summary>
        public double ValorTotal { get; set; }
        /// <summary>
        /// Tipo de movimento, exemplo: 'C' (Crédito), 'D'(Débito)
        /// </summary>
        /// 
        public string? TipoMovimento { get; set; }

        public override bool EhValido()
        {
            ValidationResult = new TransacaoCommandValidation().Validate(this);

            return ValidationResult.IsValid;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class TransacaoCommandValidation : AbstractValidator<TransacaoCommand>
    {
        private readonly int _min_length = 30;
        private readonly int _max_length = 37;
        public TransacaoCommandValidation()
        {
            RuleFor(c => c.TransacaoId)
                .NotEmpty()
                .WithMessage("{PropertyName} é requerido")
                .Length(_min_length, _max_length)
                .WithMessage("{PropertyName} inválido")
                .WithName("Código da transacao");

            RuleFor(c => c.ContaId)
                .NotEmpty()
                .WithMessage("{PropertyName} é requerido")
                .Length(_min_length, _max_length)
                .WithMessage("{PropertyName} inválido")
                .WithName("Id da conta");

            RuleFor(c => c.ValorTotal)
                .NotNull()
                .WithMessage("{PropertyName} é requerido")
                .GreaterThan(0)
                .WithMessage("O {PropertyName} precisa ser maior que {ComparisonValue}")
                .WithName("Valor");

            RuleFor(c => c.TipoMovimento)
                .NotEmpty()
                .WithMessage("{PropertyName} é requerido")
                .When(c => !string.IsNullOrEmpty(c.TipoMovimento))
                .Must(TransacaoValida)
                .WithMessage("{PropertyName} inválido")
                .WithName("Movimento");
        }

        private bool TransacaoValida(string? movimento)
        {
            if (!Enum.TryParse(movimento, out ETipoMovimento type)) return false;

            return !type.Equals(ETipoMovimento.INVALID_TYPE);
        }
    }

    public class TransacaoCommandExample : IExamplesProvider<TransacaoCommand>
    {
        public TransacaoCommand GetExamples()
        {
            return new TransacaoCommand
            {
                TransacaoId = Guid.NewGuid().ToString().ToUpper(),
                ContaId = Guid.NewGuid().ToString().ToUpper(),
                ValorTotal = 150.75,
                TipoMovimento = "C"
            };
        }
    }
}
