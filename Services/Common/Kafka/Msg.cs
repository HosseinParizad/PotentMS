namespace KafkaHelper
{
    public class Msg : IMessageContract
    {
        public Msg(string action, string belongTo, string content)
        {
            if (string.IsNullOrEmpty(action))
            {
                throw new System.ArgumentException($"'{nameof(action)}' cannot be null or empty.", nameof(action));
            }

            if (string.IsNullOrEmpty(belongTo))
            {
                throw new System.ArgumentException($"'{nameof(belongTo)}' cannot be null or empty.", nameof(belongTo));
            }

            BelongTo = belongTo;
            Content = content;
            Action = action;
        }

        public string Action { get; }
        public string Content { get; }
        public string BelongTo { get; }
    }
}