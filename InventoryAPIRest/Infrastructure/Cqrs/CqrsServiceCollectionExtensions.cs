using InventoryAPIRest.Abstractions;

namespace InventoryAPIRest.Infrastructure.Cqrs
{
    public static class CqrsServiceCollectionExtensions
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services)
        {
            var handlerContracts = new[] { typeof(ICommandHandler<,>), typeof(IQueryHandler<,>) };
            var handlers = typeof(IDispatcher).Assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false });

            foreach (var handler in handlers)
            {
                var contracts = handler.GetInterfaces()
                    .Where(i => i.IsGenericType && handlerContracts.Contains(i.GetGenericTypeDefinition()));

                foreach (var contract in contracts)
                    services.AddScoped(contract, handler);
            }

            services.AddScoped<IDispatcher, Dispatcher>();
            return services;
        }
    }
}
