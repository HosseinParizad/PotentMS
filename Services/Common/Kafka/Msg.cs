namespace KafkaHelper
{
    public class Msg : IMessageContract
    {
        public Msg(string action, string groupKey, string content)
        {
            if (string.IsNullOrEmpty(action))
            {
                throw new System.ArgumentException($"'{nameof(action)}' cannot be null or empty.", nameof(action));
            }

            if (string.IsNullOrEmpty(groupKey))
            {
                throw new System.ArgumentException($"'{nameof(groupKey)}' cannot be null or empty.", nameof(groupKey));
            }

            GroupKey = groupKey;
            Content = content;
            Action = action;
        }

        public string Action { get; }
        public string Content { get; }
        public string GroupKey { get; }
    }
}