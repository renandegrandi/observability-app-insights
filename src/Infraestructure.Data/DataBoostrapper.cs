using Domain.Repositories;
using Infraestructure.Data.Contexts.SqlServer;
using Infraestructure.Data.Repositories;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestructure.Data
{
    public static class DataBoostrapper
    {
        public static IServiceCollection AddInfraestructure(this IServiceCollection service, IConfiguration configuration) 
        {
            service.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["Redis:ConnectionString"];
                options.InstanceName = "ExemploCache";
            });

            service.AddAzureClients((builder) =>
            {
                builder.AddServiceBusClient(configuration["ServiceBus:ConnectionString"])
                    .WithName("ExemploSB");
            });

            service.AddDbContext<SqlContext>()
                .AddSqlServer<SqlContext>(configuration["SqlServer:ConnectionString"]);

            service.AddScoped<IOrderRepostory, OrderRepository>();

            return service;
        }
    }
}
