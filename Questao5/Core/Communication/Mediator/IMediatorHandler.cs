using Questao5.Core.Messages;
using Questao5.Core.Messages.CommonMessages.Notifications;

namespace Questao5.Core.Communication.Mediator
{
    public interface IMediatorHandler
    {
        Task<bool> EnviarComando<T>(T comando) where T : Command;
        Task<TEntity> EnviarComando<T, TEntity>(T comando) where T : Command;
        Task PublicarNotificacao<T>(T notificacao) where T : DomainNotification;
    }
}