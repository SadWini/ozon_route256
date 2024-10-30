using Grpc.Core;
using Grpc.Core.Interceptors;

public class ExceptionInterceptor:Interceptor
{
    private readonly ILogger<ExceptionInterceptor> _logger;

    public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
        ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (NotFoundException exc)
        {
            _logger.LogError(exc, "Couldn't find item with given id");

            throw new RpcException(new Status(StatusCode.NotFound, exc.Message));
        }
        catch (FormatException exc)
        {
            _logger.LogError(exc, $"Couldn't parse Date or Guid");
            throw new RpcException(new Status(StatusCode.InvalidArgument, String.Empty));
        }
        catch (RpcException exc)
        {
            _logger.LogError(exc, $"got status: {exc.Status}" +
                                  $"{Environment.NewLine} on method {context.Method}");
            throw;
        }
    }
}
