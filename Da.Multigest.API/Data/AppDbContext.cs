using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.Models.Tenants;
using System.Reflection;

namespace Da.Multigest.API.Data;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
	public string? CurrentTenantId { get; set; }

	public DbSet<Tenant> Tenants { get; set; }

	// On Model Creating - multitenancy query filter, fires once on app start
    protected override void OnModelCreating(ModelBuilder builder) 
	{
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
	}

	// On Save Changes - write tenant Id to table
	public override int SaveChanges()
	{
		// Write tenant Id to table
		foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>().ToList())
		{
			switch (entry.State)
			{
				case EntityState.Added:
				case EntityState.Modified:
					entry.Entity.TenantId = CurrentTenantId;
					break;
			}
		}
		var result = base.SaveChanges();
		return result;
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}