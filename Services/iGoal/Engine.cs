using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace iGoal
{
    public class Engine
    {

        #region CreateNewGoal

        public static void CreateNewGoal(dynamic metadata, dynamic content)
        {
            NewGoalItem(metadata, content, GoalType.Goal);
        }

        #endregion

        #region UpdateGoal

        public static void UpdateGoal(dynamic metadata, dynamic content)
        {
            var text = content.Text.ToString();
            var id = metadata.ReferenceKey.ToString();
            var groupkey = metadata.GroupKey.ToString();
            var goal = Goals.SingleOrDefault(t => t.Id == id);
            if (goal != null)
            {
                goal.Text = text;
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: FeedbackActions.CannotUpdateGoal, groupkey: metadata.GroupKey.ToString(), content: "Cannot update Goal");
            }

        }

        #endregion

        #region DeleteGoal

        internal static void DeleteGoal(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var Goal = Goals.SingleOrDefault(t => t.Id == id);
            var groupkey = metadata.GroupKey.ToString();
            if (Goal != null)
            {
                Goals.Remove(Goal);
                SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: FeedbackActions.GoalDeleted, groupkey: metadata.GroupKey.ToString(), content: new { Id = id });
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: FeedbackActions.CannotFindGoal, groupkey: metadata.GroupKey.ToString(), content: "Cannot find Goal item!");
            }
        }

        #endregion

        internal static IEnumerable<PresentItem> GetGoalPresentation(string groupKey, string parentid)
        {
            return Goals.Where(i => i.GroupKey == groupKey && i.ParentId == parentid).Select(i => GoalToPresentation(groupKey, i));
        }

        static PresentItem GoalToPresentation(string groupKey, GoalItem mi)
        {
            var presentItem = new PresentItem
            {
                Id = mi.Id,
                Text = mi.Text,
                Link = "",
                Actions = GetActions(mi).ToList(),
                Items = GetGoalPresentation(groupKey, mi.Id).ToList()
            };
            return presentItem;
        }

        static IEnumerable<PresentItemActions> GetActions(GoalItem mi)
        {
            Func<string, string, dynamic, PresentItemActions> createStep = (text, action, content) =>
               new PresentItemActions
               {
                   Text = text,
                   Group = "/Goal",
                   Action = action,
                   Metadata = new { GroupKey = mi.GroupKey, ReferenceKey = Guid.NewGuid().ToString() },
                   Content = content
               };
            yield return createStep("step", MapAction.Goal.NewGoal, new { Text = "[text]", ParentId = mi.Id });
            yield return createStep("update", MapAction.Goal.UpdateGoal, new { Text = "[text]", Id = mi.Id });
            yield return createStep("delete", MapAction.Goal.DelGoal, new { Id = mi.Id });
        }

        #region Implement

        static void NewGoalItem(dynamic metadata, dynamic content, GoalType goal)
        {
            var text = content.Text.ToString();
            var parentId = content.ParentId.ToString();
            var id = metadata.ReferenceKey.ToString();
            if (!Goals.Any(t => t.Id == id || (t.ParentId == parentId && t.Text == text)))
            {
                AddGoalItem(id, metadata.GroupKey.ToString(), text, parentId, GetCreateDate(metadata), goal);
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: FeedbackActions.CannotAddGoal, groupkey: metadata.GroupKey.ToString(), content: "Cannot add dupicate Goal item!");
            }
        }

        static void SendFeedbackMessage(MsgType type, string action, DateTimeOffset actionTime, string groupkey, dynamic content)
        {
            if (Program.StartingTimeApp < actionTime)
            {
                ProducerHelper.SendAMessage(
                        MessageTopic.GoalFeedback,
                        new Feedback(type: type, action: action, metadata: Helper.GetMetadataByGroupKey(groupkey), content: content)
                        )
                .GetAwaiter().GetResult();
            }
        }

        static DateTimeOffset GetCreateDate(dynamic metadata)
        {
            return DateTimeOffset.Parse(metadata.CreateDate.ToString());
        }

        static void AddGoalItem(string id, string groupKey, string text, string parentId, DateTimeOffset actionTime, GoalType GoalType)
        {
            var Goal = new GoalItem { Id = id, ParentId = parentId, GroupKey = groupKey, Text = text, GoalType = GoalType };
            Goals.Add(Goal);
            SendFeedbackMessage(type: MsgType.Success, actionTime: actionTime, action: FeedbackActions.NewGoalAdded, groupkey: groupKey, content: Goal);
        }

        #endregion

        internal static IEnumerable<GoalItem> GetGoal(string groupKey)
        {
            return Goals.Where(m => m.GroupKey == groupKey);
        }

        #region Common actions

        public static void Reset(dynamic metadata, dynamic content)
        {
            Goals = new List<GoalItem>();
        }

        public static List<GoalItem> Goals { get; private set; } = new List<GoalItem>();
        #endregion
    }
    public class GoalItem
    {
        public string Id { get; internal set; }
        public string ParentId { get; internal set; }
        public string Text { get; internal set; }
        public string GroupKey { get; internal set; }
        public GoalType GoalType { get; internal set; }
    }

    public enum GoalType
    {
        Goal,
        Category
    }
}
