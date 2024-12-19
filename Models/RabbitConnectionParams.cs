namespace EfiritPro.Retail.Packages.Rabbit.Models;

public class RabbitConnectionParams
{
    public string UserName { get; init; } = "user";
    public string Password { get; init; } = "password";
    public string VirtualHost { get; init; } = "/";
    public string HostName { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string ClientProvidedName { get; init; } = string.Empty;
    public string EventAckQueueName { get; init; } = string.Empty;

    public ICollection<string> ListenedQueueList { get; init; } = Array.Empty<string>();
}