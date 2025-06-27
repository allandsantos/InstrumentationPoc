using InstrumentationPoc.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace InstrumentationPoc;

public class AppService 
{
    private readonly ICalculatorService _calculatorService;
    private readonly IOrderService _orderService;
    private readonly ILogger<AppService> _logger;
    
    public AppService(
        ICalculatorService calculatorService, 
        IOrderService orderService,
        ILogger<AppService> logger)
    {
        _calculatorService = calculatorService;
        _orderService = orderService;
        _logger = logger;
    }
    
    public async Task RunAsync()
    {
        _logger.LogInformation("Starting application service");
        
        // Testa Calculator Service
        _logger.LogInformation("=== Testing Calculator Service ===");
        var sum = _calculatorService.Add(10, 5);
        _logger.LogInformation("Sum result: {Sum}", sum);
        
        var processedData = await _calculatorService.ProcessDataAsync("hello world");
        _logger.LogInformation("Processed data: {Data}", processedData);
        
        // Testa Order Service
        _logger.LogInformation("=== Testing Order Service ===");
        var order = await _orderService.GetOrderAsync(123);
        _logger.LogInformation("Retrieved order: {OrderId}", order.Id);
        
        await _orderService.ProcessOrderAsync(order);
        
        // Testa erro
        _logger.LogInformation("=== Testing Error Handling ===");
        try
        {
            _calculatorService.Divide(10, 0);
        }
        catch (DivideByZeroException)
        {
            _logger.LogInformation("Division by zero was handled properly");
        }
    }
}