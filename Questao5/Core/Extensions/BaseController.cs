using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Core.Communication.Mediator;
using Questao5.Core.Messages.CommonMessages.Notifications;
using Questao5.Core.Messages;

namespace Questao5.Core.Extensions
{
    [ApiController]
    public class BaseController<T> : ControllerBase
    {
        private readonly ILogger<T> _logger;
        private readonly IMediatorHandler _mediatorHandler;
        private readonly DomainNotificationHandler _notifications;

        protected BaseController(ILogger<T> logger,
                                 IMediatorHandler mediatorHandler,
                                 INotificationHandler<DomainNotification> notifications)
        {
            _logger = logger;
            _mediatorHandler = mediatorHandler;
            _notifications = (DomainNotificationHandler)notifications;
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
            {
                return Ok(result);
            }

            return BadRequest(ObterMensagensErro());
        }

        #region Notifications
        protected bool OperacaoValida()
        {
            return !_notifications.TemNotificacao();
        }

        protected IEnumerable<string> ObterMensagensErro()
        {
            return _notifications.ObterNotificacoes().Select(c => c.Value).ToList();
        }

        protected async void NotificarErro(string codigo, string mensagem)
        {
            await _mediatorHandler.PublicarNotificacao(new DomainNotification(codigo, mensagem));
        }

        protected bool ValidarComando(Command message)
        {
            if (message.EhValido()) return true;

            message.ValidationResult.Errors.ForEach(error => { NotificarErro(message.MessageType, error.ErrorMessage); });

            return false;
        }
        #endregion

        #region Log
        protected void LogInfo(string message) => _logger.LogInformation(message);

        protected void LogError(string message) => _logger.LogError(message);

        protected void LogWarning(string message) => _logger.LogWarning(message);
        #endregion
    }
}
