using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.Packages.Rabbit.Models;

public static class RabbitDbContextConfiguration
{
    public static void ConfigureRabbitDbContext(ModelBuilder builder)
    {
        builder.Entity<RabbitEvent>(entity =>
        {
            entity.ToTable("rabbit_events");
            entity.HasKey(re => re.Id);

            entity.Property(re => re.Id).ValueGeneratedOnAdd();
            entity.Property(re => re.CreateTime).ValueGeneratedOnAdd();
        });
    }
}