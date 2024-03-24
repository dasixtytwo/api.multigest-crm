using System.ComponentModel.DataAnnotations;

namespace SharedClassLibrary.DTOs.Tenants;

public class TenantRequest
{
	[Required]
	public string Name { get; set; } = String.Empty;
	[Required]
	public string SubscriptionLevel { get; set; } = String.Empty;
	public bool Isolated { get; set; } = false;
}
