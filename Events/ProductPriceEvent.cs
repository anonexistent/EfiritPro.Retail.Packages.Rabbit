using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EfiritPro.Retail.Packages.Rabbit.Events;

public class ProductPriceEvent : ProductEvent
{
    [JsonPropertyName("storeId")]
    [Required]
    public required string StoreId { get; set; }
    [JsonPropertyName("postingId")]
    [Required]
    public required string PostingId { get; set; }
    [JsonPropertyName("purchasePrice")]
    [Required]
    public required float PurchasePrice { get; set; }
    [JsonPropertyName("sellingPrice")]
    [Required]
    public required float SellingPrice { get; set; }
    [JsonPropertyName("startTime")]
    [Required]
    public required DateTime StartTime { get; set; }
}
