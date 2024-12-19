using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EfiritPro.Retail.Packages.Rabbit.Interfaces;

namespace EfiritPro.Retail.Packages.Rabbit.Events;

public class AckEvent : IRabbitEvent
{
    [JsonPropertyName("eventId")]
    [Required]
    public required string EventId { get; set; }
    [JsonPropertyName("ackDestination")] 
    [Required]
    public required string AckDestination { get; set; } = string.Empty;
}