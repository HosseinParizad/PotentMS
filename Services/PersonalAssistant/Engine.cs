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

                case FeedbackActions.TaskDeleted:
                    ApplyTaskDeleted(feedback);
                    break;

                case FeedbackActions.GoalDeleted:
                    ApplyGoalDeleted(feedback);
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
            GetDashboardSections(feedback.Key).Single(d => d.Text == "Goal").BadgesInternal = Engine.GetBadgesByGoal(feedback.Key, null).ToList();
        }

        static void ApplyNewTaskAdded(Feedback feedback)
        {
            var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            var id = data.GetProperty("Id").ToString();
            var text = data.GetProperty("Text").ToString();
            var parentId = data.GetProperty("ParentId").ToString();
            parentId = parentId == "" ? null : parentId;
            Dues.Add(new TodoItem { Text = text, Id = id, GroupKey = feedback.Key, ParentId = parentId });
            GetDashboardSections(feedback.Key).Single(d => d.Text == "Due").BadgesInternal = Engine.GetBadgesDues(feedback.Key, null).ToList();
        }

        static void ApplyUpdateTaskDescription(Feedback feedback)
        {
            var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            var id = data.GetProperty("Id").ToString();
            var text = data.GetProperty("Description").ToString();
            Dues.Single(d => d.Id == id).Text = text;
            var key = feedback.Key;
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

        static void ApplyTaskDeleted(Feedback feedback)
        {
            var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            var id = data.GetProperty("Id").ToString();
            var task = Dues.SingleOrDefault(t => t.Id == id);
            if (task != null)
            {
                Dues.Remove(task);
                GetDashboardSections(feedback.Key).Single(d => d.Text == "Due").BadgesInternal = Engine.GetBadgesDues(feedback.Key, null).ToList();
            }
        }

        static void ApplyGoalDeleted(Feedback feedback)
        {
            var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            var id = data.GetProperty("Id").ToString();
            var goal = Goals.SingleOrDefault(t => t.Id == id);
            if (goal != null)
            {
                Goals.Remove(goal);
                GetDashboardSections(feedback.Key).Single(d => d.Text == "Goal").BadgesInternal = Engine.GetBadgesByGoal(feedback.Key, null).ToList();
            }
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
                yield return new BadgeItem
                {
                    Id = task.Id,
                    Text = task.Text,
                    LinkItems = new List<LinkItem> {
                        new LinkItem { Link = JsonSerializer.Serialize(addSteps), Text = "Steps" },
                        new LinkItem { Link = JsonSerializer.Serialize(delete), Text = "Delete" },
                    },
                    Items = GetBadgesDuesTree(key, task.Id).ToList()
                };
            }
        }

        public static IEnumerable<BadgeItem> GetBadgesDuesTree(string key, string parentId)
        {
            foreach (var task in Dues.Where(t => t.GroupKey == key && parentId == t.ParentId))
            {
                yield return new BadgeItem
                {
                    Id = task.Id,
                    Text = task.Text,
                    ParentId = task.ParentId,
                    LinkItems = GetLinkItems(task, key).ToList(),
                    Items = GetBadgesDuesTree(key, task.Id).ToList()
                };
            }
        }

        public static IEnumerable<BadgeItem> GetBadgesDues(string key, string parentId)
        {
            foreach (var task in Dues.Where(t => t.GroupKey == key))
            {
                yield return new BadgeItem
                {
                    Id = task.Id,
                    Text = task.Text,
                    ParentId = task.ParentId,
                    LinkItems = GetLinkItems(task, key).ToList(),
                    Items = new List<BadgeItem>()
                };
            }
        }

        static IEnumerable<LinkItem> GetLinkItems(TodoItem task, string key)
        {
            var id = task.Id;
            var addSteps = new
            {
                Action = "newTask",
                Key = key,
                Content = JsonSerializer.Serialize(new { Description = "[text]", ParentId = id })
            };
            var delete = new
            {
                Action = "delTask",
                Key = key,
                Content = JsonSerializer.Serialize(new { Id = id })
            };
            var update = new
            {
                Action = "updateDescription",
                Key = key,
                Content = JsonSerializer.Serialize(new { Description = "[text]", Id = id })
            };
            var setLocation = new
            {
                Action = "setLocation",
                Key = key,
                Content = JsonSerializer.Serialize(new { Location = "[text]", Id = id })
            };
            var setTag = new
            {
                Action = "setTag",
                Key = key,
                Content = JsonSerializer.Serialize(new { Tag = "[text]", TagKey = 0, Id = id })
            };


            yield return new LinkItem { Link = JsonSerializer.Serialize(addSteps), Text = "Steps" };
            yield return new LinkItem { Link = JsonSerializer.Serialize(delete), Text = "Delete" };
            yield return new LinkItem { Link = JsonSerializer.Serialize(update), Text = "Update" };
            yield return new LinkItem { Link = JsonSerializer.Serialize(setLocation), Text = "Location" };
            yield return new LinkItem { Link = JsonSerializer.Serialize(setTag), Text = "Tag" };
        }
    }

    #endregion
}


