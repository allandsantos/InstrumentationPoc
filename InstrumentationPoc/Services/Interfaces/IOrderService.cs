using InstrumentationPoc.Entities;

namespace InstrumentationPoc.Services.Interfaces;

public interface IOrderService
{
    Task<Order> GetOrderAsync(int orderId);
    Task ProcessOrderAsync(Order order);
}