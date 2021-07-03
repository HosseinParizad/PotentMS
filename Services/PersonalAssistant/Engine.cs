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

                case FeedbackActions.NewGroupAdded:
                    ApplyNewGroupAdded(feedback);
                    break;

                case FeedbackActions.NewGoalAdded:
                    ApplyNewGoalAdded(feedback);
                    break;

                case FeedbackActions.NewTaskAdded:
                    ApplyNewTaskAdded(feedback);
                    break;

                case FeedbackActions.TaskAssginedToMember:
                    ApplyTaskAssginedToMember(feedback);
                    break;

                case FeedbackActions.updateTaskDescription:
                    ApplyUpdateTaskDescription(feedback);
                    break;

                default:
                    break;
            }
        }

        static void ApplyNewGroupAdded(Feedback feedback)
        {
            var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            var memberKey = data.GetProperty("MemberKey").ToString();
            var groupKey = data.GetProperty("GroupKey").ToString();

            var members = new HashSet<string>();
            if (Groups.TryGetValue(groupKey, out members))
            {
                Groups[groupKey].Add(memberKey);
            }
            else
            {
                Groups.Add(groupKey, new HashSet<string> { memberKey });
            }
        }

        static void ApplyNewGoalAdded(Feedback feedback)
        {
            var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            var id = data.GetProperty("Id").ToString();
            var goal = data.GetProperty("Goal").ToString();
            Goals.Add(new TodoItem { Text = goal, Id = id, GroupKey = feedback.Key });
            var key = feedback.Key;
            Refresh(key);
        }

        static void ApplyNewTaskAdded(Feedback feedback)
        {
            var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            var id = data.GetProperty("Id").ToString();
            var text = data.GetProperty("Text").ToString();
            var parentId = data.GetProperty("ParentId").ToString();
            parentId = parentId == "" ? null : parentId;
            Dues.Add(new TodoItem { Text = text, Id = id, GroupKey = feedback.Key, ParentId = parentId });
            var key = feedback.Key;
            Refresh(key);
        }

        static void ApplyUpdateTaskDescription(Feedback feedback)
        {
            var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            var id = data.GetProperty("Id").ToString();
            var text = data.GetProperty("Description").ToString();
            Dues.Single(d => d.Id == id).Text = text;
            var key = feedback.Key;
            Refresh(key);
        }

        static void Refresh(string key)
        {
            Dashboards.Remove(Dashboards.Single(d => d.Id == key));
            GetDashboardOrAdd(key);
        }

        static void ApplyTaskAssginedToMember(Feedback feedback)
        {
            var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            var id = data.GetProperty("Id").ToString();
            var member = data.GetProperty("MemberKey").ToString();
            TodoItem goal = Goals.SingleOrDefault(t => t.Id == id);
            if (goal != null)
            {
                goal.GroupKey = member;
            }
            TodoItem due = Dues.SingleOrDefault(t => t.Id == id);
            if (due != null)
            {
                due.GroupKey = member;
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
            //    var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            //    var key = data.GetProperty("Id").ToString();
            //    var deadline = data.GetProperty("Deadline").GetDateTimeOffset();
            //    List<BadgeItem> badges = GetDashboardSectionBadges(key, ":("); // Todo: desgin this part
            //    if (!badges.Any(b => b.Text == deadline))
            //    {
            //        badges.Add(new BadgeItem { Text = deadline });
            //    }
        }

        static List<BadgeItem> GetDashboardSectionBadges(string key, string sectionText)
            => GetDashboardSections(key).Single(d => d.Text == sectionText).BadgesInternal;

        static List<DashboardPart> GetDashboardSections(string key)
        {
            return GetDashboardOrAdd(key).Parts;
        }

        static Dashboard GetDashboardOrAdd(string key)
        {
            var dashbord = Dashboards.SingleOrDefault(d => d.Id == key);
            if (dashbord == null)
            {
                dashbord = new Dashboard(key);
                Dashboards.Add(dashbord);
            }
            return dashbord;
        }

        #region GetDashboard

        internal static IEnumerable<Dashboard> GetDashboards(string assistantKey)
            => GetDashboard(assistantKey);

        static IEnumerable<Dashboard> GetDashboard(string assistantKey)
        {
            //Dashboards.Clear();
            if (Groups.ContainsKey(assistantKey))
            {
                foreach (var member in Groups[assistantKey])
                {
                    yield return GetDashboardOrAdd(member);
                }
            }
            else
            {
                yield return GetDashboardOrAdd(assistantKey);
            }
            ///return Dashboards;
        }


        #endregion

        #region Common Action

        internal static void Reset(string arg1, string arg2)
        {
            Dashboards = new List<Dashboard>();
            Groups = new Dictionary<string, HashSet<string>>();
            Goals = new List<TodoItem>();
            Dues = new List<TodoItem>();
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

        //#region Location actions 

        //public static void SetUsedLocation(string groupKey, string content)
        //{
        //    var data = JsonSerializer.Deserialize<dynamic>(content);
        //    var key = data.GetProperty("Member").ToString();
        //    string location = data.GetProperty("Location").ToString();
        //    IEnumerable<DashboardPart> dashbord = Dashboards.Single(d => d.Id == groupKey).Parts;
        //    var locations = dashbord.Single(d => d.Text == "UsedLocations").BadgesInternal;
        //    if (!locations.Any(l => l.Text == location))
        //    {
        //        locations.Add(new BadgeItem { Text = location, Type = BadgeType.Location });
        //    }
        //}

        //#endregion

        #region Implement

        static List<Dashboard> Dashboards = new List<Dashboard>();
        public static Dictionary<string, HashSet<string>> Groups = new Dictionary<string, HashSet<string>>();

        static List<TodoItem> Goals = new List<TodoItem>();
        static List<TodoItem> Dues = new List<TodoItem>();

        public static IEnumerable<BadgeItem> GetBadgesByGoal(string key, string parentId)
        {
            foreach (var task in Goals.Where(t => t.GroupKey == key && t.ParentId == parentId))
            {
                yield return new BadgeItem
                {
                    Text = task.Text
                };
            }
        }

        public static IEnumerable<BadgeItem> GetBadgesDues(string key, string parentId)
        {
            foreach (var task in Dues.Where(t => t.GroupKey == key && t.ParentId == parentId))
            {
                //var s = $@"{  \"Action\": \"newTask\",  \"Key\": \"{this.selected.group}\",  \"Content\": \"{\"Description\":\"{this.name}\",\"ParentId\":\"{task.Id}\"}\"}";
                var addSteps = new
                {
                    Action = "newTask",
                    Key = key,
                    Content = JsonSerializer.Serialize(new { Description = "[text]", ParentId = task.Id })
                };
                var delete = new
                {
                    Action = "delTask",
                    Key = key,
                    Content = JsonSerializer.Serialize(new { Id = task.Id })
                };
                var update = new
                {
                    Action = "updateDescription",
                    Key = key,
                    Content = JsonSerializer.Serialize(new { Description = "[text]", Id = task.Id })
                };
                var setLocation = new
                {
                    Action = "setLocation",
                    Key = key,
                    Content = JsonSerializer.Serialize(new { Location = "[text]", Id = task.Id })
                };
                var setTag = new
                {
                    Action = "setTag",
                    Key = key,
                    Content = JsonSerializer.Serialize(new { Tag = "[text]", TagKey = 0, Id = task.Id })
                };

                yield return new BadgeItem
                {
                    Text = task.Text,
                    LinkItems = new List<LinkItem> {
                        new LinkItem { Link = JsonSerializer.Serialize(addSteps), Text = "Steps" },
                        new LinkItem { Link = JsonSerializer.Serialize(delete), Text = "Delete" },
                        new LinkItem { Link = JsonSerializer.Serialize(update), Text = "Update" },
                        new LinkItem { Link = JsonSerializer.Serialize(setLocation), Text = "Location" },
                        new LinkItem { Link = JsonSerializer.Serialize(setTag), Text = "Tag" },
                    },
                    Items = GetBadgesDues(key, task.Id).ToList()
                };
            }
        }

        #endregion
    }
}

