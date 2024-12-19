using EfiritPro.Retail.Packages.Rabbit.Models;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.Packages.Rabbit.Interfaces;

public interface IRabbitPublisherService<in TDbContext> where TDbContext: DbContext, IRabbitDbContext
{
    public Task<RabbitEvent> CreateRabbitEvent<TRabbitEvent>(TDbContext dbContext, string destination, TRabbitEvent body) 
        where TRabbitEvent: IRabbitEvent;
    public Task<RabbitEvent?> UpdateRabbitEventStatus(TDbContext dbContext, string id, RabbitEventStatus status);
    public Task<RabbitEvent?> UpdateRabbitEventStatus(TDbContext dbContext, Guid id, RabbitEventStatus status);
    public Task RemoveRabbitEvent(TDbContext dbContext, string id);
    public Task RemoveRabbitEvent(TDbContext dbContext, Guid id);
    public Task SendEvent(TDbContext dbContext, string id);
    public Task SendEvent(TDbContext dbContext, Guid id);
    public Task SendAck(string eventId, string destination);
}