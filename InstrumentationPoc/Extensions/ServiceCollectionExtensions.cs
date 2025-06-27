using Castle.DynamicProxy;
using InstrumentationPoc.Interceptors;
using InstrumentationPoc.Services;
using InstrumentationPoc.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace InstrumentationPoc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSerilog(this IServiceCollection services)
    {
        return services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog();
        });
    }
    
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddIntercepted<ICalculatorService, CalculatorService>();
        services.AddIntercepted<IOrderService, OrderService>();
        services.AddScoped<AppService>();

        return services;
    }
    
    
    public static IServiceCollection AddLoggingInterceptor(this IServiceCollection services)
    {
        services.AddSingleton<IProxyGenerator, ProxyGenerator>();
        services.AddSingleton<LoggingInterceptor>();

        return services;
    }

    private static IServiceCollection AddIntercepted<TInterface, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.Add(ServiceDescriptor.Describe(typeof(TImplementation), typeof(TImplementation), lifetime));
        services.Add(ServiceDescriptor.Describe(typeof(TInterface), provider =>
        {
            var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
            var interceptor = provider.GetRequiredService<LoggingInterceptor>();
            var target = provider.GetRequiredService<TImplementation>() as TInterface;
            
            return proxyGenerator.CreateInterfaceProxyWithTarget(target, interceptor);
        }, lifetime));
        
        return services;
    }
}