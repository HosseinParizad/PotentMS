using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace iTime
{
    public class Engine
    {
        #region StartTask

        public static void StartTask(dynamic metadata, dynamic content)
        {
            var applied = FeedbackActions.TimeableStarted;
            var cannotApply = FeedbackActions.CannotStartTimeable;
            var statusToApply = TimeStatus.Start;
            Func<dynamic, bool> validationCondition = (time) => time == null || time?.Status != TimeStatus.Start;
            RunAction(metadata, content, cannotApply, applied, statusToApply, validationCondition);
        }

        #endregion

        #region PauseTask

        public static void PauseTask(dynamic metadata, dynamic content)
        {
            var applied = FeedbackActions.TimeablePaused;
            var cannotApply = FeedbackActions.CannotPauseTimeable;
            var statusToApply = TimeStatus.Pause;
            Func<dynamic, bool> validationCondition = (time) => time?.Status == TimeStatus.Start;
            RunAction(metadata, content, cannotApply, applied, statusToApply, validationCondition);
        }

        #endregion

        #region DoneTask

        public static void DoneTask(dynamic metadata, dynamic content)
        {
            var applied = FeedbackActions.TimeableDone;
            var cannotApply = FeedbackActions.CannotDoneTimeable;
            var statusToApply = TimeStatus.Done;
            Func<dynamic, bool> validationCondition = (time) => time == null || time?.Status != TimeStatus.Done;
            RunAction(metadata, content, cannotApply, applied, statusToApply, validationCondition);
        }

        #endregion

        internal static IEnumerable<PresentItem> GetTimePresentation(string memberKey)
        {
            return Times.Where(i => (i.MemberKey == memberKey || i.MemberKey == "All")).Select(i => TimeToPresentation(memberKey, i));
        }

        static PresentItem TimeToPresentation(string memberKey, TimeItem mi)
        {
            var presentItem = new PresentItem
            {
                Id = mi.Id,
                Text = mi.MemberKey,
                Link = "",
                Actions = GetActions(mi).ToList(),
                Items = new()
            };
            return presentItem;
        }

        static IEnumerable<PresentItemActions> GetActions(TimeItem mi)
        {
            Func<string, string, dynamic, PresentItemActions> createStep = (text, action, content) =>
               new PresentItemActions
               {
                   Text = text,
                   Group = "/Time",
                   Action = action,
                   Metadata = new { ReferenceKey = Guid.NewGuid().ToString() },
                   Content = content
               };

            yield return createStep("Start", MapAction.Time.Start, new { ParentId = "[id]", ParentName = "Task", MemberKey = "[memberKey]" });
            yield return createStep("Pause", MapAction.Time.Pause, new { ParentId = "[id]", ParentName = "Task", MemberKey = "[memberKey]" });
            yield return createStep("Done", MapAction.Time.Done, new { ParentId = "[id]", ParentName = "Task", MemberKey = "[memberKey]" });
        }

        #region Implement

        static void RunAction(dynamic metadata, dynamic content, string cannotApply, string applied, TimeStatus statusToApply, Func<dynamic, bool> validationCondition)
        {
            var parentId = content.ParentId.ToString();
            var parentName = content.ParentName.ToString();
            var memberKey = content.MemberKey.ToString();
            var id = metadata.ReferenceKey.ToString();
            TimeItem? lastTime = LastTime(parentId);
            if (validationCondition(lastTime))
            {
                var item = new TimeItem { Id = id, ActionTime = DateTimeOffset.Now, ParentId = parentId, ParentName = parentName, Status = statusToApply, MemberKey = memberKey };
                Times.Add(item);

                SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: applied, groupkey: metadata.GroupKey.ToString(), content: item);
                if (statusToApply == TimeStatus.Start)
                {
                    //PauseOtherTask(memberKey, parentId);
                }
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: cannotApply, groupkey: metadata.GroupKey.ToString(), content: content);
            }
        }

        static List<TimeItem> FindById(string id) => Times.Where(t => t.ParentId == id).ToList();
        static TimeItem? LastTime(string id) => LastTime(FindById(id));
        static TimeItem? LastTime(IEnumerable<TimeItem> times) => times.OrderByDescending(t => t.ActionTime).LastOrDefault();

        static void SendFeedbackMessage(MsgType type, string action, DateTimeOffset actionTime, string groupkey, dynamic content)
        {
            if (Program.StartingTimeApp < actionTime)
            {
                ProducerHelper.SendAMessage(
                        MessageTopic.TimeFeedback,
                        new Feedback(type: type, action: action, metadata: Helper.GetMetadataByGroupKey(groupkey), content: content)
                        );
                //.GetAwaiter().GetResult();
            }
        }

        static DateTimeOffset GetCreateDate(dynamic metadata)
        {
            return DateTimeOffset.Parse(metadata.CreateDate.ToString());
        }

        #endregion

        //internal static IEnumerable<TimeItem> GetTime(string TimeKey)
        //{
        //    return Times.Where(m => m.TimeKey == TimeKey);
        //}

        #region Common actions

        public static void Reset(dynamic metadata, dynamic content)
        {
            Times = new List<TimeItem>();
        }

        public static List<TimeItem> Times { get; private set; } = new List<TimeItem>();
        #endregion
    }
    public class TimeItem
    {
        public string Id { get; internal set; }
        public string ParentId { get; internal set; }
        public string MemberKey { get; set; }
        public string ParentName { get; internal set; }
        public TimeStatus Status { get; set; }
        public DateTimeOffset ActionTime { get; internal set; }
        public bool AutoAdd { get; internal set; }
    }

    public enum TimeStatus
    {
        Start,
        Pause,
        Done
    }
}
