using EfiritPro.Retail.Packages.Rabbit.Models;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.Packages.Rabbit.Interfaces;

public interface IRabbitDbContext
{
    public DbSet<RabbitEvent> RabbitEvents { get; set; }
}