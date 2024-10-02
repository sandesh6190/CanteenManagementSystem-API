using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Canteen.Api.Filters;

public class GlobalExceptionFilterAttribute(
    ILogger<GlobalExceptionFilterAttribute> logger)
    : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, "An unhandled exception occurred.");

        var response = new
        {
            Message = "An unexpected error occurred. Please try again later.",
            Details = context.Exception.Message
        };

        context.Result = new JsonResult(response)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
    }
}