using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EfiritPro.Retail.Packages.Rabbit.Events;

public class InvoiceEvent : ProductInventoryEvent
{
    [JsonPropertyName("invoiceId")]
    [Required]
    public required string InvoiceId { get; set; }
}