using Asp.Versioning;
using Da.Multigest.API.Data;
using Da.Multigest.API.Repositories;
using Da.Multigest.API.Repositories.Tenants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using SharedClassLibrary.Contract;
using SharedClassLibrary.Contract.Tenants;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

namespace Da.Multigest.API.Extentions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection SetupDependencyInjection(this IServiceCollection services, IConfiguration configuration)
	{
		services
		  .SetupControllers()
		  .AddDbContext<AppDbContext>(options =>
		  {
			  options.UseSqlServer(configuration.GetConnectionString("DefaultConnection") ??
				  throw new InvalidOperationException("Connection String is not found"));
		  })
		  .SetupAuthentication(configuration)
		  .SetupSecurityPolicies(configuration)
		  .SetupSwagger()
		  .AddScoped<IUserAccount, AccountRepository>()
		  .AddScoped<ICurrentTenantService, CurrentTenantService>()
		  .AddScoped<ITenantFacade, TenantRepository>();

		return services;
	}

	private static IServiceCollection SetupControllers(this IServiceCollection services)
	{
		services.AddControllers(options =>
		{
			options.Filters.Add(new RequireHttpsAttribute());
		});
		services.AddApiVersioning(options =>
		{
			options.DefaultApiVersion = new ApiVersion(1);
			options.AssumeDefaultVersionWhenUnspecified = true;
			options.ReportApiVersions = true;
			options.ApiVersionReader = ApiVersionReader.Combine(
				new QueryStringApiVersionReader("api-version"),
				new HeaderApiVersionReader("api-version"),
				new UrlSegmentApiVersionReader()
			);
		})
          .AddApiExplorer(options =>
          {
              options.GroupNameFormat = "'v'V";
              options.SubstituteApiVersionInUrl = true;
          });

        services.AddEndpointsApiExplorer();

		return services;
	}

	private static IServiceCollection SetupAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		//Add Identity & JWT authentication, Identity
		services.AddIdentity<ApplicationUser, IdentityRole>()
		  .AddEntityFrameworkStores<AppDbContext>()
		  .AddSignInManager()
		  .AddRoles<IdentityRole>();

		services
		  .AddAuthentication(options =>
		  {
			  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		  })
		  .AddJwtBearer(options =>
		  {
			  options.TokenValidationParameters = new TokenValidationParameters
			  {
				  ValidateIssuer = true,
				  ValidateAudience = true,
				  ValidateIssuerSigningKey = true,
				  ValidateLifetime = true,
				  ValidIssuer = configuration["Jwt:Issuer"],
				  ValidAudience = configuration["Jwt:Audience"],
				  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!))
			  };
		  });

		return services;
	}

	private static IServiceCollection SetupSwagger(this IServiceCollection services)
	{

		services
		  .AddSwaggerGen(options =>
		  {
			  options.SwaggerDoc("v1", new OpenApiInfo
			  {
				  Version = "v1",
				  Title = "DA Multigest API",
				  Description = "DA Multigest OpenApi",
				  TermsOfService = new Uri("https://davideagosti.com"),
				  Contact = new OpenApiContact
				  {
					  Name = "Davide Agosti",
					  Email = string.Empty,
					  Url = new Uri("https://twitter.com/davideagosti62"),
				  },
				  License = new OpenApiLicense
				  {
					  Name = "Use under LICX",
					  Url = new Uri("https://davideagosti.com/license"),
				  }
			  });
			  options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
			  {
				  In = ParameterLocation.Header,
				  Description = "Oauth Authorization header",
				  Name = "Authorization",
				  Type = SecuritySchemeType.ApiKey
			  });
			  options.OperationFilter<SecurityRequirementsOperationFilter>();

		  });

		return services;
	}

	private static IServiceCollection SetupSecurityPolicies(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddCors(policy =>
		{
			policy.AddPolicy("AllowAll",
				builder => builder.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader()
				.WithHeaders(HeaderNames.ContentType));
		});
		return services;
	}

    #region Helper Functions

    public static string GetCorrelationId(this HttpContext httpContext)
    {
        httpContext.Request.Headers.TryGetValue("x-correlation-id", out StringValues correlationId);
        return correlationId.FirstOrDefault() ?? Guid.NewGuid().ToString();
    }

    private static T Bind<T>(this IConfiguration configuration, string sectionName) where T : new()
	{
		var bindedClass = new T();
		configuration.GetSection(sectionName).Bind(bindedClass);
		return bindedClass;
	}
	
	#endregion

}
