using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EfiritPro.Retail.Packages.Rabbit.Events;

public class OwnerEvent : AckEvent
{
    [JsonPropertyName("ownerId")]
    [Required]
    public required string OwnerId { get; set; }
}
