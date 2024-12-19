using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EfiritPro.Retail.Packages.Rabbit.Events;

public class CashReceiptEvent : ProductInventoryEvent
{
    [JsonPropertyName("cashReceiptId")]
    [Required]
    public required string CashReceiptId { get; set; }
}