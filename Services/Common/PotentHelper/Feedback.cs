using System.Diagnostics;

namespace PotentHelper
{
    //public class Feedback : Msg, IMessageContract
    //{
    //    public Feedback(string action, dynamic metadata, dynamic content) : base(action, metadata, content)
    //    {
    //    }

    //    public Feedback(MsgType type, string action, dynamic metadata, dynamic content) : base(type, action, metadata, content)
    //    {
    //    }

    //    //public Feedback(MsgType type, string action, dynamic metadata, dynamic content) : base(type, action, metadata, content)
    //    //{ }

    //    public MsgType Type { get; set; }
    //    public string Action { get; set; }
    //    public dynamic Metadata { get; set; }
    //    public dynamic Content { get; set; }
    //}
    public class Feedback : Msg
    {
        public Feedback(MsgType type, string action, dynamic metadata, dynamic content) : base(type, action, (object)metadata, (object)content)
        {
        }
    }

    public enum MsgType
    {
        Command,
        Success,
        Apply,
        Info,
        Error
    }

}
