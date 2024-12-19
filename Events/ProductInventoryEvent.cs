using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EfiritPro.Retail.Packages.Rabbit.Events;

public class ProductInventoryEvent : StoreEvent
{
    [JsonPropertyName("createTime")]
    [Required]
    public required DateTime CreateTime { get; set; }
    [JsonPropertyName("workerId")]
    [Required]
    public required string WorkerId { get; set; }
    
    [JsonPropertyName("positions")]
    [Required]
    [MinLength(0)]
    public required ICollection<ProductPosition> Positions { get; set; }
}