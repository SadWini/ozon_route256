using System;

public class LoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggerMiddleware> _logger;

    private int _count;

    public LoggerMiddleware(RequestDelegate next, 
        ILogger<LoggerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        _logger.LogInformation("New request {0}", httpContext.Request.Method);
        _count++;
        _logger.LogInformation("Request count:{0}", _count);

        try{
            await _next(httpContext);
        }
        catch(NotFoundException exc)
        {
            httpContext.Response.StatusCode = 404;
            await httpContext.Response.WriteAsync($"Problem on server side {exc}"); // change you response body if needed
        }
    }
}