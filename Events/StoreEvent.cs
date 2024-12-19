using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EfiritPro.Retail.Packages.Rabbit.Events;

public class StoreEvent : OrganizationEvent
{
    [JsonPropertyName("storeId")]
    [Required]
    public required string StoreId { get; set; }
}