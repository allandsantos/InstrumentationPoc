# .NET Log Instrumentation POC

Implementation for automatic logging in .NET applications using **Interceptor (Castle DynamicProxy)** and **custom Attributes**.

## 📋 Features

- ✅ **Declarative logging** using attributes
- ✅ **Transparent interception** via Castle DynamicProxy
- ✅ **Natural integration** with Dependency Injection
- ✅ **Full support** for synchronous and asynchronous methods
- ✅ **Flexible configuration** of service lifetimes
- ✅ **Compatible attributes** on implementation and/or interface
- ✅ **Structured logging** with Serilog

## 📖 How to Use

### 1. Define Attributes on implementation methods

```csharp
public class OrderService : IOrderService
{
    [LogExecutionTime("Retrieving order from database")]
    [LogEntryExit]
    public async Task<Order> GetOrderAsync(int orderId)
    {
        // Your logic here
        return new Order { Id = orderId };
    }
    
    [LogExecutionTime("Processing payment")]
    [LogError("Payment processing failed")]
    public async Task ProcessOrderAsync(Order order)
    {
        // Your logic here
    }
}
```

### 2. Configure Dependency Injection

```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Register services with interception
        services.AddIntercepted<IOrderService, OrderService>();
        services.AddIntercepted<ICalculatorService, CalculatorService>();
    })
    .Build();
```

### 3. Use normally via Dependency Injection

```csharp
public class OrderController
{
    private readonly IOrderService _orderService;
    
    public OrderController(IOrderService orderService) // Already instrumented!
    {
        _orderService = orderService;
    }
    
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _orderService.GetOrderAsync(id); // Automatic logging
        return Ok(order);
    }
}
```

## 🏷️ Available Attributes

### `[LogExecutionTime]`
Measures and logs method execution time.

```csharp
[LogExecutionTime("Custom description")]
public async Task ProcessDataAsync() { }
```

### `[LogError]`
Customizes error messages when exceptions occur.

```csharp
[LogError("Custom error message")]
public void RiskyOperation() { }
```

### `[LogEntryExit]`
Logs method entry and exit.

```csharp
[LogEntryExit]
public void ImportantMethod() { }
```

## 🔧 Extension Methods

### `AddIntercepted<TInterface, TImplementation>(ServiceLifetime)`
For specific lifecycle control. Optional parameter, defaults to `Scoped`.

```csharp
// Singleton for cache
services.AddIntercepted<ICacheService, CacheService>(ServiceLifetime.Singleton);

// Scoped for general services
services.AddIntercepted<IOrderService, OrderService>(ServiceLifetime.Scoped);

// Transient for isolated processing
services.AddIntercepted<IEmailSender, EmailSender>(ServiceLifetime.Transient);
```

## 📊 Sample Output

```
[14:23:45 INF] [ENTRY] Entering OrderService.GetOrderAsync
[14:23:45 DBG] [TIME] Starting Retrieving order from database
[14:23:45 INF] [TIME] Completed Retrieving order from database in 156ms
[14:23:45 INF] [EXIT] Exiting OrderService.GetOrderAsync

[14:23:46 INF] [ENTRY] Entering OrderService.ProcessOrderAsync
[14:23:46 DBG] [TIME] Starting Processing payment
[14:23:46 ERR] [ERROR] Payment processing failed in OrderService.ProcessOrderAsync after 45ms
    System.InvalidOperationException: Insufficient funds
```

## 🏗️ Architecture

```
┌─────────────────┐    ┌──────────────────┐     ┌─────────────────┐
│   Controller    │───▶│  IOrderService   │───▶│  OrderService   │
└─────────────────┘    │    (Proxy)       │     │ (Implementation)│
                       └──────────────────┘     └─────────────────┘
                               │
                               ▼
                       ┌───────────────────┐
                       │ LoggingInterceptor│
                       │ - Entry/Exit      │
                       │ - Execution Time  │
                       │ - Error Handling  │
                       └───────────────────┘
```

## 🎯 Use Cases

- **REST APIs**: Automatic logging for controllers and services
- **Console Applications**: Performance monitoring
- **Background Services**: Processing tracking
- **Microservices**: Distributed observability
- **Enterprise Applications**: Auditing and debugging

## 📝 Complete Example

See the complete source code in the main project file for a functional example with:

- Multiple services with different attributes
- Complete DI configuration
- Error handling
- Synchronous and asynchronous methods
- Serilog integration

## 🤝 Contributors

- **[Pedro Neto](https://github.com/13pneto)** - Collaborator in developing this solution

## 🆘 Troubleshooting

### Issue: Attributes are not found
**Solution**: Make sure attributes are on the implementation or interface. The interceptor is configured to search in both locations.

### Issue: Logging doesn't work
**Solution**: Verify you're using `AddIntercepted` instead of the standard `AddScoped`.

---

💡 **Tip**: This pattern is especially useful for enterprise applications where you need consistent logging without polluting business code with instrumentation logic.
