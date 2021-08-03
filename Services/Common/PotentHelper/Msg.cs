using System;

namespace PotentHelper
{
    public class Msg : IMessageContract
    {
        public Msg(string action, dynamic metadata, dynamic content)
        {
            if (string.IsNullOrEmpty(action))
            {
                throw new System.ArgumentException($"'{nameof(action)}' cannot be null or empty.", nameof(action));
            }

            if (string.IsNullOrEmpty(metadata?.ToString()))
            {
                throw new System.ArgumentException($"'{nameof(metadata)}' cannot be null or empty.", nameof(metadata));
            }

            Metadata = metadata;
            Content = content;
            Action = action;
        }

        public string Action { get; set; }
        public dynamic Metadata { get; set; }
        public dynamic Content { get; set; }
    }
}