namespace KafkaHelper
{
    public interface IMessageContract
    {
        string Action { get; }
        string BelongTo { get; }
        string Content { get; }
    }
}