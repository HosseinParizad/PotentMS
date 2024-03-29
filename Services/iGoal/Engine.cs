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
            var id = content.Id.ToString();
            var groupKey = metadata.GroupKey.ToString();
            var memberKey = metadata.MemberKey.ToString();
            var goal = Goals.SingleOrDefault(t => t.Id == id);
            if (goal != null)
            {
                goal.Text = text;
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: MapAction.GoalFeedback.CannotUpdateGoal.Name, content: "Cannot update Goal");
            }
        }

        #endregion

        #region DeleteGoal

        internal static void DeleteGoal(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var Goal = Goals.SingleOrDefault(t => t.Id == id);
            var groupKey = metadata.GroupKey.ToString();
            var memberKey = metadata.MemberKey.ToString();
            if (Goal != null)
            {
                Goals.Remove(Goal);
                SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: MapAction.GoalFeedback.GoalDeleted.Name, content: new { Id = id });
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: MapAction.GoalFeedback.CannotFindGoal.Name, content: "Cannot find Goal item!");
            }
        }

        #endregion

        internal static IEnumerable<PresentItem> GetGoalPresentation(string groupKey, string memberKey, string parentid)
        {
            return Goals.Where(i => i.GroupKey == groupKey && i.MemberKey == memberKey && i.ParentId == parentid).Select(i => GoalToPresentation(groupKey, memberKey, i));
        }

        internal static IEnumerable<PresentItem> GetTodos(string groupKey, string memberKey, string parentid)
        {
            return Goals.Where(i => i.GroupKey == groupKey && i.MemberKey == memberKey && !Goals.Any(g => g.ParentId == i.Id)).Select(i => GoalToPresentation2(groupKey, memberKey, i));
        }

        static PresentItem GoalToPresentation2(string groupKey, string memberKey, GoalItem mi)
        {
            var presentItem = new PresentItem
            {
                Id = mi.Id,
                Text = "Goal: " + mi.Text,
                Link = "",
                Actions = GetActions(mi).ToList(),
            };
            return presentItem;
        }

        static PresentItem GoalToPresentation(string groupKey, string memberKey, GoalItem mi)
        {
            var presentItem = new PresentItem
            {
                Id = mi.Id,
                Text = mi.Text,
                Link = "",
                Actions = GetActions(mi).ToList(),
                Items = GetGoalPresentation(groupKey, memberKey, mi.Id).ToList()
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
                   Metadata = new { GroupKey = mi.GroupKey, MemberKey = mi.MemberKey, ReferenceKey = Guid.NewGuid().ToString() },
                   Content = content
               };
            yield return createStep("step", MapAction.Goal.NewGoal.Name, new { Text = "[text]", ParentId = mi.Id });
            yield return createStep("update", MapAction.Goal.UpdateGoal.Name, new { Text = "[text]", Id = mi.Id });
            yield return createStep("delete", MapAction.Goal.DelGoal.Name, new { Id = mi.Id });
            yield return createStep("task", MapAction.Task.NewTask.Name, new { Description = "[text]", ParentId = mi.Id });
        }

        #region Implement

        static void NewGoalItem(dynamic metadata, dynamic content, GoalType goal)
        {
            var text = content.Text.ToString();
            var parentId = content.ParentId.ToString();
            var id = metadata.ReferenceKey.ToString();
            var groupKey = metadata.GroupKey.ToString();
            var memberKey = metadata.MemberKey.ToString();
            if (!Goals.Any(t => t.Id == id || (t.ParentId == parentId && t.Text == text)))
            {
                AddGoalItem(id, groupKey, memberKey, text, parentId, GetCreateDate(metadata), goal);
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: MapAction.GoalFeedback.CannotAddGoal.Name, content: "Cannot add dupicate Goal item!");
            }
        }

        static void SendFeedbackMessage(MsgType type, string action, DateTimeOffset actionTime, dynamic content)
        {
            if (Program.StartingTimeApp < actionTime)
            {
                ProducerHelper.SendMessage(MessageTopic.GoalFeedback, new Feedback(type: type, action: action, content: content)).GetAwaiter().GetResult();
            }
        }

        static DateTimeOffset GetCreateDate(dynamic metadata)
        {
            return DateTimeOffset.Parse(metadata.CreateDate.ToString());
        }

        static void AddGoalItem(string id, string groupKey, string memberKey, string text, string parentId, DateTimeOffset actionTime, GoalType GoalType)
        {
            var Goal = new GoalItem { Id = id, ParentId = parentId, GroupKey = groupKey, MemberKey = memberKey, Text = text, GoalType = GoalType };
            Goals.Add(Goal);
            SendFeedbackMessage(type: MsgType.Success, actionTime: actionTime, action: MapAction.GoalFeedback.NewGoalAdded.Name, content: Goal);
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
        public string MemberKey { get; internal set; }
        public GoalType GoalType { get; internal set; }
    }

    public enum GoalType
    {
        Goal,
        Category
    }
}
