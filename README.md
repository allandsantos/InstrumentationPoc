# Poc de instrumentação de logs com .NET

Implementação para logging automático em aplicações .NET usando **Interceptor(Castle DynamicProxy)** e **Attributes personalizados**.

## 📋 Características

- ✅ **Logging declarativo** usando attributes
- ✅ **Interceptação transparente** via Castle DynamicProxy
- ✅ **Integração natural** com Dependency Injection
- ✅ **Suporte completo** a métodos síncronos e assíncronos
- ✅ **Configuração flexível** de lifetime dos serviços
- ✅ **Attributes compatíveis na implementação e/ou interface**
- ✅ **Logging estruturado** com Serilog

## 📖 Como Usar

### 1. Defina os Attributes nos métodos da implementação

```csharp
public class OrderService : IOrderService
{
    [LogExecutionTime("Retrieving order from database")]
    [LogEntryExit]
    public async Task<Order> GetOrderAsync(int orderId)
    {
        // Sua lógica aqui
        return new Order { Id = orderId };
    }
    
    [LogExecutionTime("Processing payment")]
    [LogError("Payment processing failed")]
    public async Task ProcessOrderAsync(Order order)
    {
        // Sua lógica aqui
    }
}
```

### 2. Configure a Injeção de Dependência

```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Registrar serviços com interceptação
        services.AddIntercepted<IOrderService, OrderService>();
        services.AddIntercepted<ICalculatorService, CalculatorService>();
    })
    .Build();
```

### 3. Use normalmente via Dependency Injection

```csharp
public class OrderController
{
    private readonly IOrderService _orderService;
    
    public OrderController(IOrderService orderService) // Já instrumentado!
    {
        _orderService = orderService;
    }
    
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _orderService.GetOrderAsync(id); // Logging automático
        return Ok(order);
    }
}
```

## 🏷️ Attributes Disponíveis

### `[LogExecutionTime]`
Mede e registra o tempo de execução do método.

```csharp
[LogExecutionTime("Custom description")]
public async Task ProcessDataAsync() { }
```

### `[LogError]`
Personaliza mensagens de erro quando exceções ocorrem.

```csharp
[LogError("Custom error message")]
public void RiskyOperation() { }
```

### `[LogEntryExit]`
Registra entrada e saída dos métodos.

```csharp
[LogEntryExit]
public void ImportantMethod() { }
```

## 🔧 Extension Methods

### `AddIntercepted<TInterface, TImplementation>(ServiceLifetime)`
Para controle específico do ciclo de vida. Parâmetro opcional, por padrão `Scoped`.

```csharp
// Singleton para cache
services.AddIntercepted<ICacheService, CacheService>(ServiceLifetime.Singleton);

// Scoped para serviços em geral
services.AddIntercepted<IOrderService, OrderService>(ServiceLifetime.Scoped);

// Transient para processamento isolado
services.AddIntercepted<IEmailSender, EmailSender>(ServiceLifetime.Transient);
```

## 📊 Exemplo de Saída

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

## 🏗️ Arquitetura

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

## 🎯 Casos de Uso

- **APIs REST**: Logging automático de controllers e serviços
- **Aplicações Console**: Monitoramento de performance
- **Background Services**: Rastreamento de processamento
- **Microservices**: Observabilidade distribuída
- **Aplicações Enterprise**: Auditoria e debugging

## 📝 Exemplo Completo

Veja o código fonte completo no arquivo principal do projeto para um exemplo funcional com:

- Múltiplos serviços com diferentes attributes
- Configuração completa de DI
- Tratamento de erros
- Métodos síncronos e assíncronos
- Integração com Serilog

## 🤝 Colaboradores

- **[Pedro Neto](https://github.com/13pneto)** - Colaborador no desenvolvimento desta solução

## 🆘 Troubleshooting

### Problema: Attributes não são encontrados
**Solução**: Certifique-se de que os attributes estão na implementação ou na interface. O interceptor foi configurado para buscar em ambos os locais.

### Problema: Logging não funciona
**Solução**: Verifique se você está usando `AddIntercepted` em vez do `AddScoped` padrão.

---

💡 **Dica**: Este padrão é especialmente útil para aplicações enterprise onde você precisa de logging consistente sem poluir o código de negócio com lógica de instrumentação.
