using Da.Multigest.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.Contract.Tenants;
using SharedClassLibrary.DTOs.Tenants;
using SharedClassLibrary.Models.Tenants;

namespace Da.Multigest.API.Repositories.Tenants;

public class TenantRepository(AppDbContext _context, IConfiguration _configuration, IServiceProvider _serviceProvider) : ITenantFacade
{
  public async Task<Tenant> GetTenantByNameAsync(string tenantName)
  {
    var response = await _context.Tenants.Where(n => n.Name == tenantName).FirstOrDefaultAsync();

    if (response != null)
    {
      return response;
    }
    else
    {
      throw new Exception("Tenant not found");
    }
  }

  public async Task<Tenant> GetTenantByIdAsync(string tenantId)
  {
    var response = await _context.Tenants.Where(n => n.Id == tenantId).FirstOrDefaultAsync();

    if (response != null)
    {
      return response;
    }
    else
    {
      throw new Exception("Tenant not found");

    }
  }

  public async Task<Tenant> CreateNewTenantAsync([FromBody] TenantRequest request)
  {
    string newConnectionString = null;
    if (request.Isolated == true)
    {
      // generate a connection string for new tenant database
      string dbName = "multiTenantAppDb-" + request.Name;
      string defaultConnectionString = _configuration.GetConnectionString("DefaultConnection");
      newConnectionString = defaultConnectionString.Replace("multiTenantAppDb", dbName);

      // create a new tenant database and bring current with any pending migrations from ApplicationDbContext
      try
      {
        using IServiceScope scopeTenant = _serviceProvider.CreateScope();
        AppDbContext dbContext = scopeTenant.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.SetConnectionString(newConnectionString);
        if (dbContext.Database.GetPendingMigrations().Any())
        {
          Console.ForegroundColor = ConsoleColor.Blue;
          Console.WriteLine($"Applying ApplicationDB Migrations for New '{request.Name}' tenant.");
          Console.ResetColor();
          dbContext.Database.Migrate();
        }
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    var response = await _context.Tenants.Where(n => n.Name == request.Name).FirstOrDefaultAsync();
    
    if (response != null)
    {
      return response;
    }

    // create a new tenant entity
    Tenant tenant = new()
    {
      Id = Guid.NewGuid().ToString(),
      Name = request.Name,
      SubscriptionLevel = request.SubscriptionLevel
    };

    await _context.Tenants.AddAsync(tenant);
    _context.SaveChanges();

    return tenant;
  }
}
