using Microsoft.AspNetCore.Diagnostics;
using WebApi.Models;

namespace WebApi.ServiceExtensions;

public class AppExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, errorMessage) = exception switch
        {
            ResponseException ex => (ex.StatusCode, ex.Message),
            _ => (500, "Something Went Wrong")
        };

        var response = new Response(errorMessage, statusCode);
        await response.ExecuteResultHttpAsync(httpContext);

        return true;
    }
}