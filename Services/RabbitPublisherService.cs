using System.Text.Json;
using EfiritPro.Retail.Packages.Rabbit.Events;
using EfiritPro.Retail.Packages.Rabbit.Interfaces;
using EfiritPro.Retail.Packages.Rabbit.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace EfiritPro.Retail.Packages.Rabbit.Services;

public class RabbitPublisherService<TDbContext>: IRabbitPublisherService<TDbContext> where TDbContext : DbContext, IRabbitDbContext
{
    private readonly RabbitConnectionParams _connectionParams;
    private const string RecognizerToken = "_recognizer-token";

    public RabbitPublisherService(RabbitConnectionParams connectionParams)
    {
        _connectionParams = connectionParams;
    }

    public async Task<RabbitEvent> CreateRabbitEvent<TRabbitEvent>(TDbContext dbContext, string destination, TRabbitEvent body)
        where TRabbitEvent : IRabbitEvent
    {
        body.EventId = RecognizerToken;
        body.AckDestination = _connectionParams.EventAckQueueName;

        var rabbitEvent = new RabbitEvent()
        {
            Destination = destination,
            Body = JsonSerializer.Serialize<TRabbitEvent>(body),
            CreateTime = DateTime.UtcNow
        };

        await dbContext.RabbitEvents.AddAsync(rabbitEvent);

        return rabbitEvent;
    }
    
    public async Task<RabbitEvent?> GetRabbitEvent(TDbContext dbContext, string id) =>
        await GetRabbitEvent(dbContext, Guid.Parse(id));

    public async Task<RabbitEvent?> GetRabbitEvent(TDbContext dbContext, Guid id) =>
        await dbContext.RabbitEvents.FirstOrDefaultAsync(re => re.Id == id);

    public async Task<ICollection<RabbitEvent>> GetRabbitEventList(TDbContext dbContext, RabbitEventStatus? status, string? destination,
        DateTime? dateFrom, DateTime? dateTo)
    {
        var list = dbContext.RabbitEvents.AsQueryable();

        if (status is not null) list = list.Where(re => re.Status == status);
        if (destination is not null) list = list.Where(re => re.Destination == destination);
        if (dateFrom is not null) list = list.Where(re => re.CreateTime >= dateFrom);
        if (dateTo is not null) list = list.Where(re => re.CreateTime <= dateTo);

        return await list.ToArrayAsync();
    }

    public async Task<RabbitEvent?> UpdateRabbitEventStatus(TDbContext dbContext, string id, RabbitEventStatus status) =>
        await UpdateRabbitEventStatus(dbContext,Guid.Parse(id), status);

    public async Task<RabbitEvent?> UpdateRabbitEventStatus(TDbContext dbContext, Guid id, RabbitEventStatus status)
    {
        var rabbitEvent = await GetRabbitEvent(dbContext, id);

        if (rabbitEvent is not null)
        {
            rabbitEvent.Status = status;
            dbContext.RabbitEvents.Update(rabbitEvent);
        }
        
        return rabbitEvent;
    }

    public async Task RemoveRabbitEvent(TDbContext dbContext, string id) =>
        await RemoveRabbitEvent(dbContext, Guid.Parse(id));

    public async Task RemoveRabbitEvent(TDbContext dbContext, Guid id)
    {
        var rabbitEvent = await GetRabbitEvent(dbContext,id);

        if (rabbitEvent is not null) dbContext.RabbitEvents.Remove(rabbitEvent);
    }

    public async Task SendEvent(TDbContext dbContext, string id) =>
        await SendEvent(dbContext, Guid.Parse(id));
    
    public async Task SendEvent(TDbContext dbContext, Guid id)
    {
        var rabbitEvent = await GetRabbitEvent(dbContext, id);

        if (rabbitEvent is not null)
        {
            try
            {
                var connection = new ConnectionFactory()
                {
                    UserName = _connectionParams.UserName,
                    Password = _connectionParams.Password,
                    VirtualHost = _connectionParams.VirtualHost,
                    HostName = _connectionParams.HostName,
                    Port = _connectionParams.Port,
                    ClientProvidedName = _connectionParams.ClientProvidedName
                }.CreateConnection();
                var channel = connection.CreateModel();
                channel.QueueDeclare(queue: rabbitEvent.Destination,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var message = System.Text.Encoding.UTF8.GetBytes(rabbitEvent.Body.Replace(RecognizerToken, rabbitEvent.Id.ToString()));
                channel.BasicPublish(string.Empty,  rabbitEvent.Destination, null, message);

                await UpdateRabbitEventStatus(dbContext, id, RabbitEventStatus.Sent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
    
    public async Task SendAck(string eventId, string destination)
    {
        try
        {
            var connection = new ConnectionFactory()
            {
                UserName = _connectionParams.UserName,
                Password = _connectionParams.Password,
                VirtualHost = _connectionParams.VirtualHost,
                HostName = _connectionParams.HostName,
                Port = _connectionParams.Port,
                ClientProvidedName = _connectionParams.ClientProvidedName
            }.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: destination,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var message = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new AckEvent()
            {
                EventId = eventId,
                AckDestination = destination,
            }));
            channel.BasicPublish(string.Empty, destination, null, message);
            // channel.Close();
            // connection.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}