using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EfiritPro.Retail.Packages.Rabbit.Events;

public class CreateOrganizationEvent : OrganizationEvent
{
    [JsonPropertyName("loginPrefix")]
    [Required]
    public required string LoginPrefix { get; set; }
}