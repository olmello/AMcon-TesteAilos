using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Handlers;
using Questao5.Core.Communication.Mediator;
using Questao5.Core.Messages.CommonMessages.Notifications;
using Questao5.Domain;
using Questao5.Infrastructure.Services;
using System.Reflection;

namespace Questao5.Setup
{
    public static partial class DependencyInjection
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            services.AddSingleton<DatabaseService>();

            services.AddScoped<IRequestHandler<ExisteTransacaoCommand, bool>, TransacaoIdempotenciaHandler>();
            services.AddScoped<IRequestHandler<AdicionarTransacaoCommand, bool>, TransacaoIdempotenciaHandler>();
            services.AddScoped<IRequestHandler<AdicionarResultadoTransacaoCommand, bool>, TransacaoIdempotenciaHandler>();
            services.AddScoped<IRequestHandler<ObterResultadoTransacaoCommand, string>, TransacaoIdempotenciaHandler>();

            services.AddScoped<IContaService, ContaService>();
            services.AddScoped<IContaRepository, ContaRepository>();
            services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();
        }
    }
}
