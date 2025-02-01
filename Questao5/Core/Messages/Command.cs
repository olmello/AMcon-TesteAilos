using FluentValidation.Results;
using MediatR;
using System.Text.Json.Serialization;

namespace Questao5.Core.Messages
{
    public abstract class Command : Message, IRequest<bool>
    {
        [JsonIgnore]
        public DateTime Timestamp { get; set; }
        [JsonIgnore]
        public ValidationResult ValidationResult { get; set; }

        protected Command()
        {
            Timestamp = DateTime.Now;
        }

        public virtual bool EhValido()
        {
            throw new NotImplementedException();
        }
    }
}