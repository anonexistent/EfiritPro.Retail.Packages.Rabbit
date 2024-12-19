namespace EfiritPro.Retail.Packages.Rabbit.Models;

public enum RabbitEventStatus
{
    Created,
    Sent,
    Completed,
}

public class RabbitEvent
{
    public Guid Id { get; set; }
    public RabbitEventStatus Status { get; set; } = RabbitEventStatus.Created;
    public string Destination { get; set; }
    public string Body { get; set; }
    public DateTime CreateTime { get; set; }
}