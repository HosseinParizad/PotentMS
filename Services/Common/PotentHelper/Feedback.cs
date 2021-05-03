using System;
namespace PotentHelper
{
    public class Feedback
    {
        public Feedback()
        {

        }

        public Feedback(FeedbackType type = default, string groupKey = null, string id = null, string messege = null, string originalRequest = null)
        {
            Type = type;
            GroupKey = groupKey;
            Id = id;
            Messege = messege;
            OriginalRequest = originalRequest;
        }

        public FeedbackType Type { get; set; }
        public string GroupKey { get; set; }
        public string Id { get; set; }
        public string Messege { get; set; }
        public string OriginalRequest { get; set; }
    }

    public enum FeedbackType
    {
        Success,
        Info,
        Error
    }
}
