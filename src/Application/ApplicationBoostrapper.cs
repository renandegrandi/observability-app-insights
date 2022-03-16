using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationBoostrapper
    {
        public static IServiceCollection AddApplication(this IServiceCollection service) 
        {
            service.AddScoped<IOrderService, OrderService>();

            return service;
        }
    }
}