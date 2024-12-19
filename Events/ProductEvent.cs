using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EfiritPro.Retail.Packages.Rabbit.Events;

public class ProductEvent : OrganizationEvent
{
    [JsonPropertyName("productId")]
    [Required]
    public required string ProductId { get; set; }
}