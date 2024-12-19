using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EfiritPro.Retail.Packages.Rabbit.Events;

public class OrganizationEvent : OwnerEvent
{    
    [JsonPropertyName("organizationId")]
    [Required]
    public required string OrganizationId { get; set; }
}