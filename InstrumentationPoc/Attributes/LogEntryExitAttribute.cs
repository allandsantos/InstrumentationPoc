namespace InstrumentationPoc.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class LogEntryExitAttribute : Attribute { }