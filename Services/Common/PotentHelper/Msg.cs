using Newtonsoft.Json;
using System;

namespace PotentHelper
{

    public class MsgX
    {
        public MsgX() { }

        public MsgType Type { get; set; }
        public string Action { get; set; }
        public string Metadata { get; set; }
        public string Content { get; set; }
    }

    public class Msg : IMessageContract
    {
        public Msg() { }

        public Msg(MsgX c)
        {
            Type = c.Type;
            Content = c.Content;
            Metadata = c.Metadata;
            Action = c.Action;
        }

        public Msg(MsgType type, string action, dynamic metadata, dynamic content) : this(action, (object)metadata, (object)content)
        {
            Type = type;
        }
        public Msg(MsgType type, MapActionItem action, dynamic metadata, dynamic content) : this(type, action.Name, (object)metadata, (object)content)
        {
        }

        public Msg(MapActionItem action, dynamic metadata, dynamic content) : this(action.Name, (object)metadata, (object)content)
        { }

        public Msg(string action, dynamic metadata, dynamic content)
        {
            if (string.IsNullOrEmpty(action))
            {
                throw new ArgumentException($"'{nameof(action)}' cannot be null or empty.", nameof(action));
            }

            if (string.IsNullOrEmpty(metadata?.ToString()))
            {
                throw new ArgumentException($"'{nameof(metadata)}' cannot be null or empty.", nameof(metadata));
            }

            Type = MsgType.Command;
            Metadata = metadata;
            Content = content;
            Action = action;
        }

        public MsgType Type { get; set; }
        public string Action { get; set; }
        public dynamic Metadata { get; set; }
        public dynamic Content { get; set; }

        //public override string ToString()
        //{
        //    return JsonConvert.SerializeObject(this);
        //}

        public void ToFix()
        {
            Metadata = JsonConvert.DeserializeAnonymousType<dynamic>(Metadata.ToString(), Metadata);
            Content = JsonConvert.DeserializeAnonymousType<dynamic>(Content.ToString(), Content);
        }
    }

    public class FullMessage
    {
        public FullMessage(string topic, Msg message)
        {
            Topic = topic;
            Message = message;
        }
        public string Topic { get; set; }
        public Msg Message { get; set; }
    }
}