using InventoryAPIRest.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryAPIRest.Infrastructure.Cqrs
{
    public class Dispatcher : IDispatcher
    {
        private readonly IServiceProvider _provider;

        public Dispatcher(IServiceProvider provider) => _provider = provider;

        public Task<TResult> SendAsync<TResult>(ICommand<TResult> command)
        {
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
            dynamic handler = _provider.GetRequiredService(handlerType);
            return handler.HandleAsync((dynamic)command);
        }

        public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic handler = _provider.GetRequiredService(handlerType);
            return handler.HandleAsync((dynamic)query);
        }
    }
}
