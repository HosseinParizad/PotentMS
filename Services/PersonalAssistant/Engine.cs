using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using PotentHelper;

namespace PersonalAssistant
{

    internal class Engine
    {
        internal static void OnTaskFeedback(Feedback feedback)
        {
            if (feedback.Action == FeedbackActions.NewTagAdded)
            {
                IEnumerable<DashboardItem> dashbord = GetDashboard(feedback.Key);
                var badges = dashbord.Single(d => d.Text == "Tag").Badges;
                if (!badges.Contains(feedback.Content))
                {
                    badges.Add(feedback.Content);
                }
            }

            if (feedback.Action == FeedbackActions.DeadlineUpdated)
            {
                List<DeadlineItem> deadlines;
                if (!Deadlines.TryGetValue(feedback.Key, out deadlines))
                {
                    deadlines = new List<DeadlineItem>();
                    Deadlines.Add(feedback.Key, deadlines);
                }

                var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
                var id = data.GetProperty("Id").ToString();
                var text = data.GetProperty("Text").ToString();
                var date = data.GetProperty("Deadline").GetDateTimeOffset();
                var deadline = deadlines.Where(d => d.Id == id);
                if (!deadline.Any())
                {
                    deadlines.Add(new DeadlineItem { Id = id, Text = text, Deadline = date });
                }
                else
                {
                    deadline.Single().Text = text;
                    deadline.Single().Deadline = date;
                }
            }
        }

        #region GetDashboard

        public static IEnumerable<DashboardItem> GetDashboard(string assistantKey)
        {
            var result = Dashboards.Where(i => i.AssistantKey == assistantKey);
            if (!result.Any())
            {
                Dashboards.Add(DashboardItemGoal(assistantKey));
                Dashboards.Add(DashboardItemTag(assistantKey));
            }
            return result.OrderBy(i => i.Sequence);

        }

        static DashboardItem DashboardItemGoal(string assistantKey)
            => new DashboardItem { AssistantKey = assistantKey, Text = "Goal", Description = "Aim to do short or long term!", Id = assistantKey, Sequence = 0, Badges = new List<string> { "Deadlines" } };

        static DashboardItem DashboardItemTag(string assistantKey)
            => new DashboardItem { AssistantKey = assistantKey, Text = "Tag", Description = "Tag should be able to get task or sort by selecting tag, e.g i am in shop now!", Id = assistantKey, Sequence = 1 };

        #endregion

        #region Common Action

        internal static void Reset(string arg1, string arg2)
        {
            Dashboards = new List<DashboardItem>();
            Deadlines = new Dictionary<string, List<DeadlineItem>>();
        }

        #endregion


        #region Implement

        //static void SendFeedbackMessage(FeedbackType type, string groupKey, string id, string message, string originalRequest)
        //    => ProducerHelper.SendAMessage("PersonalAssistantFeedback", JsonSerializer.Serialize(new Feedback(type: type, action: "PersonalAssistantFeedback", groupKey: groupKey, content: message))).GetAwaiter().GetResult();

        static List<DashboardItem> Dashboards = new List<DashboardItem>();
        static Dictionary<string, List<DeadlineItem>> Deadlines = new Dictionary<string, List<DeadlineItem>>();

        internal static IEnumerable<DeadlineItem> GetDeadlines(string assistantKey)
        {
            return Deadlines[assistantKey].OrderBy(l => l.Deadline);
        }

        #endregion
    }
}

