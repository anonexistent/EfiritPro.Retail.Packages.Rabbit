using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EfiritPro.Retail.Packages.Rabbit.Events;

public class ProductPosition
{
    [JsonPropertyName("productId")]
    [Required]
    public required string ProductId { get; set; } 
    [JsonPropertyName("productCount")]
    [Required]
    public required float ProductCount { get; set; }
}