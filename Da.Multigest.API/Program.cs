using Da.Multigest.API.Extentions;
using Da.Multigest.API.Middleware;
using DA.Multigest.API.Extensions;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;

services.SetupDependencyInjection(builder.Configuration);

builder.Host.UseSerilog((context, logConf) => logConf
  .Enrich.FromLogContext()
  .Enrich.WithCorrelationIdHeader("x-correlation-id")
  .Enrich.WithProperty("ApplicationName", "Da Multigest API")
  .ReadFrom.Configuration(context.Configuration)
  .WriteTo.BetterStack(
      sourceToken: "oFMPcWRap5j9GBcS8SZbmCG6",
      queueLimitBytes: 100 * (1024 * 1024),
      batchSize: 100,
      restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error
      )
  .WriteTo.Console(
    theme: AnsiConsoleTheme.Code,
    outputTemplate: 
        "[{Timestamp:dd/mm/yyyy HH:mm:ss} {Level: u3} <cId:{CorrelationId}>] {Message:lj} <sc:{SourceContext}>{NewLine}{Exception}")
  );

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
app.AddHandlerManagerMiddleware();
app.UseStaticFiles();

app.MapControllers();

app.Run();
