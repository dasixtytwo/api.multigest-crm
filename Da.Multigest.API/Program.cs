using Da.Multigest.API.Extentions;
using Da.Multigest.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;

services.SetupDependencyInjection(builder.Configuration);

// builder the web app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
  app.UseSwaggerUI(options =>
    {
      options.InjectStylesheet("/swagger-ui/custom.css");
    });
  app.UseCors("AllowAll");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantResolver>();

app.UseStaticFiles();

app.MapControllers();

app.Run();
