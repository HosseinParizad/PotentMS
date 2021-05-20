namespace PotentHelper
{
    public interface IMessageContract
    {
        string Action { get; }
        string Key { get; }
        string Content { get; }
    }
}