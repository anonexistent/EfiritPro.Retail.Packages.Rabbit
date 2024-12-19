using System.Text.Json.Serialization;

namespace EfiritPro.Retail.Packages.Rabbit.Events;

public class SendEmailEvent : AckEvent
{
    [JsonPropertyName("message")]
    public required string Message { get; set; }
    [JsonPropertyName("title")]
    public required string Title { get; set; }
    [JsonPropertyName("emails")]
    public required ICollection<string> Emails { get; set; }
}
