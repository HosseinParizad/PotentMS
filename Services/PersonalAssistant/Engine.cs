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
                var newTag = feedback.Content;
                List<BadgeItem> badges = GetDashboardSectionBadges(feedback.Key, "Tag");
                if (!badges.Any(b => b.Text == newTag))
                {
                    badges.Add(new BadgeItem { Text = newTag });
                }
            }

            if (feedback.Action == FeedbackActions.NewLocationAdded)
            {
                var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
                var key = data.GetProperty("Member").ToString();
                var location = data.GetProperty("Location").ToString();
                List<BadgeItem> badges = GetDashboardSectionBadges(key, "UsedLocations");
                if (!badges.Any(b => b.Text == location))
                {
                    badges.Add(new BadgeItem { Text = location });
                }
            }

            if (feedback.Action == FeedbackActions.DeadlineUpdated)
            {
                var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
                var key = data.GetProperty("Id").ToString();
                var deadline = data.GetProperty("Deadline").GetDateTimeOffset();
                List<BadgeItem> badges = GetDashboardSectionBadges(key, ":("); // Todo: desgin this part
                if (!badges.Any(b => b.Text == deadline))
                {
                    badges.Add(new BadgeItem { Text = deadline });
                }
            }
        }

        static List<BadgeItem> GetDashboardSectionBadges(string key, string sectionText)
            => GetDashboardSections(key).Single(d => d.Text == sectionText).Badges;

        #region GetDashboard

        internal static IEnumerable<DashboardPart> GetDashboardSections(string assistantKey)
            => GetDashboard(assistantKey).Parts.OrderBy(p => p.Sequence);

        static Dashboard GetDashboard(string assistantKey)
        {
            var dashboard = Dashboards.SingleOrDefault(i => i.AssistantKey == assistantKey);
            if (dashboard == null)
            {
                dashboard = new Dashboard(assistantKey);
                Dashboards.Add(dashboard);
            }

            return dashboard;
        }


        #endregion

        #region Common Action

        internal static void Reset(string arg1, string arg2)
        {
            Dashboards = new List<Dashboard>();
            //Deadlines = new Dictionary<string, List<DeadlineItem>>();
        }

        #endregion

        #region Location actions 

        public static void SetCurrentLocation(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var key = data.GetProperty("Member").ToString();
            string location = data.GetProperty("Location").ToString();
            Console.WriteLine("<><><><><>>3<<>");
            Console.WriteLine(key);
            Console.WriteLine(location);
            Dashboard dashbord = GetDashboard(key);
            dashbord.CurrentLocation = location;
            Console.WriteLine("<><><><><>>4<<>", data);
        }

        #endregion

        #region Location actions 

        public static void SetUsedLocation(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var key = data.GetProperty("Member").ToString();
            string location = data.GetProperty("Location").ToString();
            IEnumerable<TodosItem> dashbord = GetDashboardSections(key);
            var locations = dashbord.Single(d => d.Text == "UsedLocations").UsedLocations;
            if (!locations.Contains(location))
            {
                locations.Add(location);
            }
        }

        #endregion

        #region Implement

        //static void SendFeedbackMessage(FeedbackType type, string groupKey, string id, string message, string originalRequest)
        //    => ProducerHelper.SendAMessage("PersonalAssistantFeedback", JsonSerializer.Serialize(new Feedback(type: type, action: "PersonalAssistantFeedback", groupKey: groupKey, content: message))).GetAwaiter().GetResult();

        static List<Dashboard> Dashboards = new List<Dashboard>();
        //static Dictionary<string, List<DeadlineItem>> Deadlines = new Dictionary<string, List<DeadlineItem>>();
        static Dictionary<string, List<string>> Locations = new Dictionary<string, List<string>>();

        //internal static IEnumerable<DeadlineItem> GetDeadlines(string assistantKey)
        //{
        //    return Deadlines[assistantKey].OrderBy(l => l.Deadline);
        //}

        #endregion
    }
}

