using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Core.Communication.Mediator;
using Questao5.Core.Extensions;
using Questao5.Core.Messages.CommonMessages.Notifications;
using Questao5.Infrastructure.Database.QueryStore.Responses;
using System.ComponentModel.DataAnnotations;

namespace Questao5.Infrastructure.Services.Controllers
{
    [Route("[controller]")]
    [Consumes("application/json")]
    public class AccountController : BaseController
    {
        private readonly IContaService _contaService;
        public AccountController(IContaService contaService,
                                 IMediatorHandler mediatorHandler,
                                 INotificationHandler<DomainNotification> notifications) : base(mediatorHandler, notifications)
        {
                _contaService = contaService;
        }

        /// <summary>
        /// Efetua uma transação em conta corrente do cliente, crédito ou débito
        /// </summary>
        /// <response code="200">Transação efetuada com sucesso</response>
        /// <response code="400">Ocorreu um problema, verificar a mensagem de erro</response>
        /// <response code="500">Não foi possível efetuar a transação no momento</response>

        [HttpPost]
        [Route("efetuar-transacao")]
        public async Task<IActionResult> Transacao(TransacaoCommand transacao)
        {
            if (!ValidarComando(transacao)) return CustomResponse();

            string movimentoId = await _contaService.EfetuarTransacaoAsync(transacao);

            return CustomResponse(movimentoId);
        }

        /// <summary>
        /// Obtém o extrato do cliente no momento atual
        /// </summary>
        /// <response code="200">Extrato obtido com sucesso</response>
        /// <response code="400">Ocorreu um problema, verificar a mensagem de erro</response>
        /// <response code="500">Não foi possível obter as informações no momento</response>
        /// <example>09003037-530B-4324-B9D2-4F6968728D14</example>

        [HttpGet]
        [Route("obter-extrato")]
        public async Task<IActionResult> Movimentacao([Required]string contaId)
        {
            if (string.IsNullOrEmpty(contaId))
            {
                NotificarErro("obter-extrato", "O Id da conta é requerido");

                return CustomResponse();
            }

            ExtratoBancarioQuery extrato = await _contaService.ObterExtratoPorId(contaId);

            return CustomResponse(extrato);
        }
    }
}