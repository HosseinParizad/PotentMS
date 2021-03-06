using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace iAssistant
{
    public class Engine
    {
        internal static IEnumerable<PresentItem> GetPresentation(string groupKey, string parentid)
        {
            return Todos.Where(i => i.GroupKey == groupKey && i.ParentId == parentid).OrderBy(t => t.Location == MemberLocation[t.GroupKey] ? 0 : 1).Select(i => ToPresentation(groupKey, i));
        }

        static PresentItem ToPresentation(string groupKey, TodoItem mi)
        {
            var presentItem = new PresentItem
            {
                Id = mi.Id,
                Text = mi.Text,
                Link = ""
                //Actions = GetActions(mi).ToList(),
                //Items = GetPresentation(groupKey, mi.Id).ToList()
            };
            return presentItem;
        }

        internal static void OnNewTaskAdded(dynamic metadata, dynamic content)
        {
            Todos.Add(new TodoItem
            {
                Id = content.Id.ToString(),
                Text = content.Text.ToString(),
                ParentId = content.ParentId.ToString(),
                GroupKey = metadata.GroupKey.ToString()
            });
            var groupKey = metadata.GroupKey.ToString();
            if (!MemberLocation.ContainsKey(groupKey))
            {
                MemberLocation.Add(groupKey, "");
            }

        }

        internal static void MemberSetLocation(dynamic metadata, dynamic content)
        {
            //var groupKey = metadata.GroupKey.ToString();
            var location = content.Location.ToString();
            var id = content.Id.ToString();
            var todo = Todos.SingleOrDefault(t => t.Id == id);
            if (todo != null)
            {
                todo.Location = location;
            }
        }

        static Dictionary<string, string> MemberLocation = new();

        internal static void MemberMoved(dynamic metadata, dynamic content)
        {
            var groupKey = metadata.GroupKey.ToString();
            var newLocation = content.NewLocation.ToString();
            if (MemberLocation.ContainsKey(groupKey))
            {
                MemberLocation[groupKey] = newLocation;
            }
            else
            {
                MemberLocation.Add(groupKey, newLocation);
            }
        }

        //static IEnumerable<PresentItemActions> GetActions(GoalItem mi)
        //{
        //    Func<string, string, dynamic, PresentItemActions> createStep = (text, action, content) =>
        //       new PresentItemActions
        //       {
        //           Text = text,
        //           Group = "/Goal",
        //           Action = action,
        //           Metadata = new { GroupKey = mi.GroupKey, ReferenceKey = Guid.NewGuid().ToString() },
        //           Content = content
        //       };
        //    yield return createStep("step", MapAction.Goal.NewGoal, new { Text = "[text]", ParentId = mi.Id });
        //    yield return createStep("update", MapAction.Goal.UpdateGoal, new { Text = "[text]", Id = mi.Id });
        //    yield return createStep("delete", MapAction.Goal.DelGoal, new { Id = mi.Id });
        //}

        #region Implement

        //static void SendFeedbackMessage(MsgType type, string action, DateTimeOffset actionTime, string groupkey, dynamic content)
        //{
        //    if (Program.StartingTimeApp < actionTime)
        //    {
        //        ProducerHelper.SendAMessage(
        //                MessageTopic.GoalFeedback,
        //                new Feedback(type: type, action: action, metadata: Helper.GetMetadataByGroupKey(groupkey), content: content)
        //                )
        //        .GetAwaiter().GetResult();
        //    }
        //}

        static DateTimeOffset GetCreateDate(dynamic metadata)
        {
            return DateTimeOffset.Parse(metadata.CreateDate.ToString());
        }


        #endregion

        #region Common actions

        public static void Reset(dynamic metadata, dynamic content)
        {
            Todos = new();
        }

        static List<TodoItem> Todos = new();

        #endregion
    }
    public class TodoItem
    {
        public string Id { get; internal set; }
        public string Text { get; internal set; }
        public string GroupKey { get; internal set; }
        public string ParentId { get; internal set; }
        public string Location { get; internal set; }
    }

}
