using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
	private readonly ILogger<GlobalExceptionFilter> _logger;

	public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
	{
		_logger = logger;
	}

	public void OnException(ExceptionContext context)
	{
		_logger.LogError(context.Exception, "Unhandled exception occurred.");

		var result = new
		{
			message = "An internal server error occurred. Please try again later.",
			error = context.Exception.Message,
			exceptionType = context.Exception.GetType().Name
		};

		context.Result = new JsonResult(result)
		{
			StatusCode = StatusCodes.Status500InternalServerError
		};

		context.ExceptionHandled = true;
	}
}
