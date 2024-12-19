using EfiritPro.Retail.Packages.Rabbit.Interfaces;
using EfiritPro.Retail.Packages.Rabbit.Models;
using EfiritPro.Retail.Packages.Rabbit.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EfiritPro.Retail.Packages.Rabbit.Extensions;

public static class RabbitEntryPointExtensions
{
    public static IServiceCollection AddRabbit<TDbContext, TRabbitEventHandler>(this IServiceCollection services)
        where TDbContext: DbContext, IRabbitDbContext
        where TRabbitEventHandler: class, IRabbitEventHandler
    {
        var rabbitUserName = Environment.GetEnvironmentVariable("RABBIT_USER_NAME") ??
                             throw new InvalidOperationException("string RABBIT_USER_NAME not found.");
        var rabbitPassword = Environment.GetEnvironmentVariable("RABBIT_PASSWORD") ??
                             throw new InvalidOperationException("string RABBIT_PASSWORD not found.");
        var rabbitVirtHost = Environment.GetEnvironmentVariable("RABBIT_VIRT_HOST") ??
                             throw new InvalidOperationException("string RABBIT_VIRT_HOST not found.");
        var rabbitHostName = Environment.GetEnvironmentVariable("RABBIT_HOST_NAME") ??
                             throw new InvalidOperationException("string RABBIT_HOST_NAME not found.");
        var rabbitHostPort = Environment.GetEnvironmentVariable("RABBIT_HOST_PORT") ?? 
                             throw new InvalidOperationException("string RABBIT_HOST_PORT not found.");
        var rabbitProvidedName = Environment.GetEnvironmentVariable("RABBIT_PROVIDED_NAME")
                                 ?? throw new InvalidOperationException("RABBIT_PROVIDED_NAME string not found");
        var rabbitAckQueue = Environment.GetEnvironmentVariable("RABBIT_ACK_QUEUE")
                             ?? throw new InvalidOperationException("RABBIT_ACK_QUEUE string not found");
        var rabbitListenedQueues = Environment.GetEnvironmentVariable("RABBIT_LISTENED_QUEUES") 
                                   ?? throw new InvalidOperationException("RABBIT_LISTENED_QUEUES string not found");



        var rabbitConnectionParams = new RabbitConnectionParams()
        {
            UserName = rabbitUserName,
            Password = rabbitPassword,
            VirtualHost = rabbitVirtHost,
            HostName = rabbitHostName,
            Port = int.Parse(rabbitHostPort),
            ClientProvidedName = rabbitProvidedName,
            EventAckQueueName = rabbitAckQueue,
            ListenedQueueList = rabbitListenedQueues.Split(';'),
        };
        
        services
            .AddSingleton<RabbitConnectionParams>(rabbitConnectionParams)
            .AddSingleton<IRabbitPublisherService<TDbContext>, RabbitPublisherService<TDbContext>>()
            .AddScoped<IRabbitEventHandler, TRabbitEventHandler>()
            .AddHostedService<RabbitListenerService<TDbContext>>();

        return services;
    }
}