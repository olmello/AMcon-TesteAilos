using MediatR;
using Questao5.Core.Messages;
using Questao5.Core.Messages.CommonMessages.Notifications;

namespace Questao5.Core.Communication.Mediator
{
    public class MediatorHandler : IMediatorHandler
    {
        private readonly IMediator _mediator;

        public MediatorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> EnviarComando<T>(T comando) where T : Command
        {
            return await _mediator.Send(comando);
        }

        public async Task<TEntity> EnviarComando<T, TEntity>(T comando) where T : Command
        {
            return await _mediator.Send((IRequest<TEntity>)comando);
        }
        

        public async Task PublicarNotificacao<T>(T notificacao) where T : DomainNotification
        {
            await _mediator.Publish(notificacao);
        }
    }
}
