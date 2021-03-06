using Domain.Repositories;
using Infraestructure.Data.Contexts.MongoDB;
using Infraestructure.Data.Contexts.SqlServer;
using Infraestructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.ApplicationInsights.DependencyInjection;

namespace Infraestructure.Data
{
    public static class DataBoostrapper
    {
        public static IServiceCollection AddInfraestructure(this IServiceCollection service, IConfiguration configuration) 
        {
            service.AddStackExchangeRedisCache(options =>
            {                
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "ExemploCache";
            });

            service.AddAzureClients((builder) =>
            {
                builder.AddServiceBusClient(configuration.GetConnectionString("ServiceBus"))
                    .WithName("ExemploSB");
            });

            service.AddDbContext<SqlContext>((options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            });

            service.AddMongoClient(configuration.GetConnectionString("MongoDB"));

            service.AddSingleton<IMongoContext, MongoContext>();

            service.AddScoped<IOrderRepostory, OrderRepository>();

            return service;
        }
    }
}
