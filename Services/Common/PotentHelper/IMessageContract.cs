namespace PotentHelper
{
    public interface IMessageContract
    {
        string Action { get; }
        string GroupKey { get; }
        string Content { get; }
    }
}