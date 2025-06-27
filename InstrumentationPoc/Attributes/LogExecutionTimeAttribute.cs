namespace InstrumentationPoc.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class LogExecutionTimeAttribute : Attribute
{
    public string Description { get; set; }
    public LogExecutionTimeAttribute(string description = null) => Description = description;
}