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
            return Todos.Where(i => i.GroupKey == groupKey && i.ParentId == parentid).Select(i => ToPresentation(groupKey, i));
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
            //          var id = feedback.Content.Id.ToString();
            //          {
            //              {
            //                  "Id": "47399CA8-C533-413C-A2A9-BAF5CDB85AE3",
            //"Text": "Watch cat videos on YouTube all day",
            //"ParentId": ""
            // }
            //          }
            //          {
            //              {
            //                  "GroupKey": "Ali",
            //"ReferenceKey": "efd17aca-c3d3-44a0-8e52-57b691e643c2",
            //"CreateDate": "2021-09-18T07:43:13.7794312+10:00",
            //"Version": "V0.0"
            //             }
            //          }
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
    }

}
