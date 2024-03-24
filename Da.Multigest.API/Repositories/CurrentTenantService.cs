using Da.Multigest.API.Data;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.Contract;
using SharedClassLibrary.Contract.Tenants;
using SharedClassLibrary.DTOs.Tenants;

namespace Da.Multigest.API.Repositories;

public class CurrentTenantService : ICurrentTenantService
{
	private readonly AppDbContext _context;
	private readonly ITenantFacade _tenantFacade;


	public CurrentTenantService(AppDbContext context, ITenantFacade tenantFacade)
	{
		_context = context;
		_tenantFacade = tenantFacade;
	}

	public async Task<bool> SetTenant(string tenant)
	{
		var tenantRequest = new TenantRequest()
		{
			Name = tenant,
			SubscriptionLevel = "free7",
			Isolated = false,
		};

		var tenantResponse = await _tenantFacade.CreateNewTenantAsync(tenantRequest);

		// Check if tenant is in Guid format
		bool isValid = Guid.TryParse(tenantResponse.Id, out _);
		if (!isValid) { throw new Exception("Tenant is not in correct format"); }

		// check if tenant exists
		var tenantInfo = await _context.Tenants.Where(x => x.Id == tenantResponse.Id).FirstOrDefaultAsync();

		if (tenantInfo != null)
		{
			TenantId = tenant;
			return true;
		}
		else
		{
			throw new Exception("Tenant invalid");
		}
	}

	public string? TenantId { get; set; }
}
