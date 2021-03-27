namespace KafkaHelper
{
    public interface IMessageContract
    {
        string BelongTo { get; }
        string Content { get; }
    }
}