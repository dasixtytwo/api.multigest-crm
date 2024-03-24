using SharedClassLibrary.Contract;

namespace Da.Multigest.API.Middleware;

public class TenantResolver
{
  private readonly RequestDelegate _next;
  public TenantResolver(RequestDelegate next)
  {
    _next = next;
  }

  // Get Tenant Id from incoming requests 
  public async Task InvokeAsync(HttpContext context, ICurrentTenantService currentTenantService)
  {
    // Tenant Id from incoming request header
    context.Request.Headers.TryGetValue("tenant", out var tenantFromHeader);
    if (string.IsNullOrEmpty(tenantFromHeader) == false)
    {
      await currentTenantService.SetTenant(tenantFromHeader);
    }

    await _next(context);
  }
}
