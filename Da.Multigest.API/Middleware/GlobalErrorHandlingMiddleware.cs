using Da.Multigest.API.Extentions;
using DA.Multigest.API.Entities.HandlerException;
using DA.Multigest.API.Exceptions;
using Serilog.Context;
using System.Net;
using System.Text.Json;

using KeyNotFoundException = DA.Multigest.API.Exceptions.KeyNotFoundException;
using NotImplementedException = DA.Multigest.API.Exceptions.NotImplementedException;
using UnauthorizedAccessException = DA.Multigest.API.Exceptions.UnauthorizedAccessException;

namespace DA.Multigest.API.Middleware;


public class GlobalErrorHandlingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

	public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var correlationId = context.GetCorrelationId();

		try
		{
			_logger.LogInformation($"Begin Handler Exception. {DateTime.Now}");
			using (LogContext.PushProperty("CorrelationId", correlationId))
			{
			  await _next.Invoke(context);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, ex.Message);
			using (LogContext.PushProperty("CorrelationId", correlationId))
			{
			  await HandleExceptionAsync(context, ex);
			}
		}
	} 
	
	private async Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		context.Response.ContentType = "application/json";
		var response = context.Response;
		ResponseExceptionModel exModel = new ResponseExceptionModel();

		switch (exception)
		{
			case BadRequestException ex:
				exModel.responseCode = (int)HttpStatusCode.BadRequest;
				response.StatusCode = (int)HttpStatusCode.BadRequest;
				exModel.responseMessage = ex.Message;
				break;
			case NotFoundException ex:
				exModel.responseCode = (int)HttpStatusCode.NotFound;
				response.StatusCode = (int)HttpStatusCode.NotFound;
				exModel.responseMessage = ex.Message;
				break;
			case NotImplementedException ex:
				exModel.responseCode = (int)HttpStatusCode.NotImplemented;
				response.StatusCode = (int)HttpStatusCode.NotImplemented;
				exModel.responseMessage = ex.Message;
				break;
			case UnauthorizedAccessException ex:
				exModel.responseCode = (int)HttpStatusCode.Unauthorized;
				response.StatusCode = (int)HttpStatusCode.Unauthorized;
				exModel.responseMessage = ex.Message;
				break;
			case KeyNotFoundException ex:
				exModel.responseCode = (int)HttpStatusCode.Unauthorized;
				response.StatusCode = (int)HttpStatusCode.Unauthorized;
				exModel.responseMessage = ex.Message;
				break;
			default:
				exModel.responseCode = (int)HttpStatusCode.BadRequest;
				response.StatusCode = (int)HttpStatusCode.BadRequest;
				exModel.responseMessage = "Internal Server Error, Please retry after sometime";
				break;
		}

		var exceptionResult = JsonSerializer.Serialize(exModel);
		await context.Response.WriteAsync(exceptionResult);
	}
}

