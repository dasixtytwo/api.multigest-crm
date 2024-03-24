using Microsoft.AspNetCore.Identity;
using SharedClassLibrary.Models.Tenants;
using System.ComponentModel.DataAnnotations.Schema;

namespace Da.Multigest.API.Data;

public class ApplicationUser : IdentityUser
{
	public string? Name { get; set; }
  [ForeignKey("Tenant")]
  public string? TenantId { get; set; }
  public virtual Tenant? Tenant { get; set; }
}
