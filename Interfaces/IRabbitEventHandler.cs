namespace EfiritPro.Retail.Packages.Rabbit.Interfaces;

public interface IRabbitEventHandler
{
    public Task HandleEvent(string queue, string eventBody);
}