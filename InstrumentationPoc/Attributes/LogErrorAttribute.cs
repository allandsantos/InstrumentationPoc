namespace InstrumentationPoc.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class LogErrorAttribute : Attribute
{
    public string ErrorMessage { get; set; }
    public LogErrorAttribute(string errorMessage = null) => ErrorMessage = errorMessage;
}