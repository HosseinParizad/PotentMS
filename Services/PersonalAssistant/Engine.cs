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
                dashbord.Single(d => d.Text == "Tag").Badges.Add(feedback.Content);
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
            => new DashboardItem { AssistantKey = assistantKey, Text = "Goal", Description = "Adim to do short or long term", Id = assistantKey, Sequence = 0 };

        static DashboardItem DashboardItemTag(string assistantKey)
            => new DashboardItem { AssistantKey = assistantKey, Text = "Tag", Description = "Tag should be able to get task or sort by selecting tag, e.g i am in shop now!", Id = assistantKey, Sequence = 1 };

        #endregion


        #region Implement

        //static void SendFeedbackMessage(FeedbackType type, string groupKey, string id, string message, string originalRequest)
        //    => ProducerHelper.SendAMessage("PersonalAssistantFeedback", JsonSerializer.Serialize(new Feedback(type: type, action: "PersonalAssistantFeedback", groupKey: groupKey, content: message))).GetAwaiter().GetResult();

        static List<DashboardItem> Dashboards = new List<DashboardItem>();

        #endregion
    }
}

