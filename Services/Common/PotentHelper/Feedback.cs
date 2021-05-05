using System;
namespace PotentHelper
{
    public class Feedback
    {
        public Feedback()
        {

        }

        public Feedback(FeedbackType type = default, string groupKey = null, string id = null, string message = null, string originalRequest = null)
        {
            Type = type;
            GroupKey = groupKey;
            Id = id;
            Message = message;
            OriginalRequest = originalRequest;
        }

        public FeedbackType Type { get; set; }
        public string GroupKey { get; set; }
        public string Id { get; set; }
        public string Message { get; set; }
        public string OriginalRequest { get; set; }
    }

    public enum FeedbackType
    {
        Success,
        Info,
        Error
    }
}
