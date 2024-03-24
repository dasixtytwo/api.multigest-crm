using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SharedClassLibrary.Models.Tenants;

public class Tenant
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SubscriptionLevel { get; set; }
}
