using InstrumentationPoc.Attributes;
using InstrumentationPoc.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace InstrumentationPoc.Services;

[LogEntryExit]
public class CalculatorService : ICalculatorService
{
    private readonly ILogger<CalculatorService> _logger;
    
    public CalculatorService(ILogger<CalculatorService> logger)
    {
        _logger = logger;
    }
    
    [LogExecutionTime("Adding two numbers")]
    public int Add(int a, int b)
    {
        _logger.LogDebug("Performing addition: {A} + {B}", a, b);
        Thread.Sleep(100);
        return a + b;
    }
    
    [LogExecutionTime("Division operation")]
    [LogError("Division failed")]
    public int Divide(int a, int b)
    {
        Thread.Sleep(50);
        if (b == 0)
            throw new DivideByZeroException("Cannot divide by zero");
        return a / b;
    }
    
    [LogExecutionTime("Processing async data")]
    public async Task<string> ProcessDataAsync(string data)
    {
        await Task.Delay(200);
        if (string.IsNullOrEmpty(data))
            throw new ArgumentException("Data cannot be empty");
        return $"Processed: {data.ToUpper()}";
    }
}