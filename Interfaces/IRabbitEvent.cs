namespace EfiritPro.Retail.Packages.Rabbit.Interfaces;

public interface IRabbitEvent
{
    public string EventId { get; set; }
    public string AckDestination { get; set; }
}