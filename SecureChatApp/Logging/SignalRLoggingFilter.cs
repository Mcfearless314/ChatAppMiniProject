using Microsoft.AspNetCore.SignalR;

namespace SecureChatApp.Logging;

public class SignalRLoggingFilter : IHubFilter
{
    private readonly ILogger<SignalRLoggingFilter> _logger;

    public SignalRLoggingFilter(ILogger<SignalRLoggingFilter> logger)
    {
        _logger = logger;
    }

    public async ValueTask<object> InvokeMethodAsync(
        HubInvocationContext context, Func<HubInvocationContext, ValueTask<object>> next)
    {
        _logger.LogInformation("SignalR Method Invoked: {MethodName}, Arguments: {Arguments}", 
            context.HubMethodName, context.HubMethodArguments);

        try
        {
            var result = await next(context);
            _logger.LogInformation("SignalR Method {MethodName} completed successfully", context.HubMethodName);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SignalR Method: {MethodName}", context.HubMethodName);
            throw;
        }
    }
}