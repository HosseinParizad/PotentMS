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
            switch (feedback.Action)
            {
                case FeedbackActions.NewTagAdded:
                    ApplyNewTagAdded(feedback);
                    break;

                case FeedbackActions.NewLocationAdded:
                    ApplyLocationAdded(feedback);
                    break;
                case FeedbackActions.DeadlineUpdated:
                    ApplyDeadlineUpdated(feedback);
                    break;
                default:
                    break;
            }
        }

        static void ApplyNewTagAdded(Feedback feedback)
        {
            var newTag = feedback.Content;
            List<BadgeItem> badges = GetDashboardSectionBadges(feedback.Key, "Tag");
            if (!badges.Any(b => b.Text == newTag))
            {
                badges.Add(new BadgeItem { Text = newTag });
            }
        }

        static void ApplyLocationAdded(Feedback feedback)
        {
            var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            var key = feedback.Key;
            var location = data.GetProperty("Location").ToString();
            List<BadgeItem> badges = GetDashboardSectionBadges(key, "UsedLocations");
            if (!badges.Any(b => b.Text == location))
            {
                badges.Add(new BadgeItem { Text = location, Type = BadgeType.Location });
            }
        }

        static void ApplyDeadlineUpdated(Feedback feedback)
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

        static List<BadgeItem> GetDashboardSectionBadges(string key, string sectionText)
            => GetDashboardSections(key).Single(d => d.Text == sectionText).BadgesInternal;

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
            Dashboard dashbord = GetDashboard(key);
            dashbord.CurrentLocation = location;
        }

        #endregion

        #region Location actions 

        public static void SetUsedLocation(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var key = data.GetProperty("Member").ToString();
            string location = data.GetProperty("Location").ToString();
            IEnumerable<DashboardPart> dashbord = GetDashboardSections(groupKey);
            var locations = dashbord.Single(d => d.Text == "UsedLocations").BadgesInternal;
            if (!locations.Any(l => l.Text == location))
            {
                locations.Add(new BadgeItem { Text = location, Type = BadgeType.Location });
            }
        }

        #endregion

        #region Implement

        static List<Dashboard> Dashboards = new List<Dashboard>();

        #endregion
    }
}

