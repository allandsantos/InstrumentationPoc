using InstrumentationPoc.Attributes;
using InstrumentationPoc.Entities;
using InstrumentationPoc.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace InstrumentationPoc.Services;

public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    
    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
    }
    
    [LogExecutionTime("Retrieving order")]
    [LogEntryExit]
    public async Task<Order> GetOrderAsync(int orderId)
    {
        await Task.Delay(150);
        return new Order { Id = orderId, CustomerName = "John Doe", Amount = 99.99m };
    }
    
    [LogExecutionTime("Processing order")]
    [LogError("Order processing failed")]
    public async Task ProcessOrderAsync(Order order)
    {
        await Task.Delay(300);
        _logger.LogInformation("Processing order {OrderId} for amount {Amount}", order.Id, order.Amount);
    }
}