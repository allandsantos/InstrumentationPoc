namespace InstrumentationPoc.Services.Interfaces;

public interface ICalculatorService
{
    int Add(int a, int b);
    int Divide(int a, int b);
    Task<string> ProcessDataAsync(string data);
}