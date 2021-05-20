namespace PotentHelper
{
    public class Msg : IMessageContract
    {
        public Msg(string action, string key, string content)
        {
            if (string.IsNullOrEmpty(action))
            {
                throw new System.ArgumentException($"'{nameof(action)}' cannot be null or empty.", nameof(action));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new System.ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
            }

            Key = key;
            Content = content;
            Action = action;
        }

        public string Action { get; set; }
        public string Content { get; set; }
        public string Key { get; set; }
    }
}