namespace PotentHelper
{
    public interface IMessageContract
    {
        string Action { get; }
        dynamic Metadata { get; }
        dynamic Content { get; }
    }
}