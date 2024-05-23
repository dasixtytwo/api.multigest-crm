using DA.Multigest.API.Middleware;

namespace DA.Multigest.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder AddHandlerManagerMiddleware(this IApplicationBuilder applicationBuilder)
        => applicationBuilder.UseMiddleware<GlobalErrorHandlingMiddleware>();
}
