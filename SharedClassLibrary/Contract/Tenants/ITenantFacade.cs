using SharedClassLibrary.DTOs.Tenants;
using SharedClassLibrary.Models.Tenants;

namespace SharedClassLibrary.Contract.Tenants
{
	public interface ITenantFacade
	{
		Task<Tenant> GetTenantByNameAsync(string tenantName);
		Task<Tenant> GetTenantByIdAsync(string tenantId);
		Task<Tenant> CreateNewTenantAsync(TenantRequest request);
	}
}
