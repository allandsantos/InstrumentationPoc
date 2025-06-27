using System.Diagnostics;
using System.Reflection;
using Castle.DynamicProxy;
using InstrumentationPoc.Attributes;
using Microsoft.Extensions.Logging;

namespace InstrumentationPoc.Interceptors;

public class LoggingInterceptor : IInterceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;
    
    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }
    
    public void Intercept(IInvocation invocation)
    {
        var method = invocation.Method;
        var methodName = $"{invocation.TargetType.Name}.{method.Name}";
        
        var logExecutionTime = GetMethodAttribute<LogExecutionTimeAttribute>(invocation);
        var logError = GetMethodAttribute<LogErrorAttribute>(invocation);
        var logEntryExit = GetMethodAttribute<LogEntryExitAttribute>(invocation);
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            if (logEntryExit != null)
                _logger.LogInformation("[ENTRY] Entering {MethodName}", methodName);
            
            if (logExecutionTime != null)
            {
                var description = logExecutionTime.Description ?? methodName;
                _logger.LogDebug("[TIME] Starting {Description}", description);
            }
            
            invocation.Proceed();
            
            stopwatch.Stop();
            
            if (logExecutionTime != null)
            {
                var description = logExecutionTime.Description ?? methodName;
                _logger.LogInformation("[TIME] Completed {Description} in {ElapsedMs}ms", 
                    description, stopwatch.ElapsedMilliseconds);
            }
            
            if (logEntryExit != null)
                _logger.LogInformation("[EXIT] Exiting {MethodName}", methodName);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            if (logError != null)
            {
                var errorMsg = logError.ErrorMessage ?? "Error occurred";
                _logger.LogError(ex, "[ERROR] {ErrorMessage} in {MethodName} after {ElapsedMs}ms", 
                    errorMsg, methodName, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogError(ex, "[ERROR] Unhandled error in {MethodName} after {ElapsedMs}ms", 
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            
            throw;
        }
    }
    
    // Método para buscar attributes tanto na interface quanto na implementação
    private T GetMethodAttribute<T>(IInvocation invocation) where T : Attribute
    {
        var interfaceMethod = invocation.Method;
        
        // 1. Primeiro tenta buscar na interface
        var attribute = interfaceMethod.GetCustomAttribute<T>();
        if (attribute != null) return attribute;
        
        // 2. Se não encontrou, busca na implementação
        var targetType = invocation.TargetType;
        var implementationMethod = targetType.GetMethod(
            interfaceMethod.Name,
            interfaceMethod.GetParameters().Select(p => p.ParameterType).ToArray());
            
        if (implementationMethod != null)
        {
            attribute = implementationMethod.GetCustomAttribute<T>();
            if (attribute != null) return attribute;
        }
        
        // 3. Se ainda não encontrou, busca na classe da implementação
        return targetType.GetCustomAttribute<T>();
    }
}