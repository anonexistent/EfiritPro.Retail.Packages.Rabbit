using System.Text.Json;
using EfiritPro.Retail.Packages.Rabbit.Events;
using EfiritPro.Retail.Packages.Rabbit.Interfaces;
using EfiritPro.Retail.Packages.Rabbit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EfiritPro.Retail.Packages.Rabbit.Services;

public class RabbitListenerService<TDbContext> : BackgroundService where TDbContext: DbContext, IRabbitDbContext
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;

    private readonly RabbitConnectionParams _connectionParams;

    public RabbitListenerService(IServiceProvider serviceProvider, RabbitConnectionParams connectionParams)
    {
        _serviceProvider = serviceProvider;
        _connectionParams = connectionParams;

        _connection = new ConnectionFactory()
        {
            UserName = _connectionParams.UserName,
            Password = _connectionParams.Password,
            VirtualHost = _connectionParams.VirtualHost,
            HostName = _connectionParams.HostName,
            Port = _connectionParams.Port,
            ClientProvidedName = _connectionParams.ClientProvidedName
        }.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        _channel.QueueDeclare(queue: _connectionParams.EventAckQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        foreach (var queue in _connectionParams.ListenedQueueList)
        {
            _channel.QueueDeclare(queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        
        Console.WriteLine("RabbitMQ start listen");
        
        var ackConsumer = new EventingBasicConsumer(_channel);
        ackConsumer.Received += HandleAckEvent;
        _channel.BasicConsume(queue: _connectionParams.EventAckQueueName,
            autoAck: true,
            consumer: ackConsumer);
        
        
        foreach (var queue in _connectionParams.ListenedQueueList)
        {
            var eventConsumer = new EventingBasicConsumer(_channel);
            eventConsumer.Received += (model, ea) => HandleEvent(model, ea, queue);
            _channel.BasicConsume(queue: queue,
                autoAck: true,
                consumer: eventConsumer);
        }

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("RabbitMQ end listen");
        _connection.Close();
        _channel.Close();
        
        return base.StopAsync(cancellationToken);
    }

    private void HandleAckEvent(object? model, BasicDeliverEventArgs ea)
    {
        try
        {
            var body = ea.Body.Span;
            var eventObj = JsonSerializer.Deserialize<AckEvent>(body);
            if (eventObj is null) throw new Exception("Входные данные неправильны");
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            var task = scope.ServiceProvider
                .GetRequiredService<IRabbitPublisherService<TDbContext>>()
                .UpdateRabbitEventStatus(dbContext, eventObj.EventId, RabbitEventStatus.Completed);
            task.Wait();
            dbContext.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void HandleEvent(object? model, BasicDeliverEventArgs ea, string queue)
    {
        try
        {
            var body = System.Text.Encoding.UTF8.GetString(ea.Body.Span);
            using var scope = _serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<IRabbitEventHandler>().HandleEvent(queue, body).Wait();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public override void Dispose()
    {
        Console.WriteLine("RabbitMQ end listen");
        _connection.Close();
        _channel.Close();
        base.Dispose();
    }
}