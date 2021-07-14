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

                case FeedbackActions.TaskStarted:
                    ApplyStartTask(feedback);
                    break;

                case FeedbackActions.TaskPaused:
                    ApplyPauseTask(feedback);
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
            var goal = data.GetProperty(GoalSectionKey).ToString();
            var item = new TodoItem { Text = goal, Id = id, GroupKey = feedback.Key };
            Goals.Add(item);
            Tasks.Add(item);
            var key = feedback.Key;
            GetDashboardSections(feedback.Key).Single(d => d.Text == GoalSectionKey).BadgesInternal = Engine.GetBadgesByGoal(feedback.Key, null).ToList();
        }

        static void ApplyNewTaskAdded(Feedback feedback)
        {
            var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            var id = data.GetProperty("Id").ToString();
            var text = data.GetProperty("Text").ToString();
            var parentId = data.GetProperty("ParentId").ToString();
            parentId = parentId == "" ? null : parentId;
            var item = new TodoItem { Text = text, Id = id, GroupKey = feedback.Key, ParentId = parentId };
            Tasks.Add(item);
            var parent = Tasks.SingleOrDefault(d => d.Id == parentId);
            if (parent != null)
            {
                parent.IsParent = true;
            }
            OnTaskChanged(feedback.Key);
        }

        static void ApplyUpdateTaskDescription(Feedback feedback)
        {
            var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            var id = data.GetProperty("Id").ToString();
            var text = data.GetProperty("Description").ToString();
            Tasks.Single(d => d.Id == id).Text = text;
            var key = feedback.Key;
            OnTaskChanged(feedback.Key);
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
            var task = Tasks.SingleOrDefault(t => t.Id == id);
            if (task != null)
            {
                task.GroupKey = member;
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
            var id = data.GetProperty("Id").ToString();
            var deadline = data.GetProperty("Deadline").GetDateTimeOffset();
            var task = Tasks.Single(t => t.Id == id);
            if (task != null)
            {
                task.Deadline = deadline;
            }
            OnTaskChanged(feedback.Key);
        }

        static void ApplyTaskDeleted(Feedback feedback)
        {
            var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            var id = data.GetProperty("Id").ToString();
            TodoItem task = Tasks.SingleOrDefault(t => t.Id == id);
            if (task != null)
            {
                Tasks.Remove(task);
                if (!string.IsNullOrEmpty(task.ParentId))
                {
                    var parent = Tasks.Single(t => t.Id == task.ParentId);
                    if (!Tasks.Any(t => t.ParentId == parent.Id))
                    {
                        parent.IsParent = false;
                    }
                }
                OnTaskChanged(feedback.Key);
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
                GetDashboardSections(feedback.Key).Single(d => d.Text == GoalSectionKey).BadgesInternal = Engine.GetBadgesByGoal(feedback.Key, null).ToList();
            }
            var task = Tasks.SingleOrDefault(t => t.Id == id);
            if (task != null)
            {
                Tasks.Remove(goal);
                GetDashboardSections(feedback.Key).Single(d => d.Text == TaskSectionKey).BadgesInternal = Engine.GetBadgesTasks(feedback.Key, null).ToList();
            }
        }

        static void ApplyStartTask(Feedback feedback)
        {
            var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            var id = data.GetProperty("Id").ToString();
            Tasks.Single(d => d.Id == id).Status = TodoStatus.start;
            var key = feedback.Key;
            OnTaskChanged(feedback.Key);
        }

        static void ApplyPauseTask(Feedback feedback)
        {
            var data = JsonSerializer.Deserialize<dynamic>(feedback.Content);
            var id = data.GetProperty("Id").ToString();
            Tasks.Single(d => d.Id == id).Status = TodoStatus.pause;
            var key = feedback.Key;
            OnTaskChanged(feedback.Key);
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
            Tasks = new List<TodoItem>();
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

        const string GoalSectionKey = "Goal";
        const string DueSectionKey = "Due";
        const string TaskSectionKey = "Task";

        static void OnTaskChanged(string key)
        {
            GetDashboardSections(key).Single(d => d.Text == DueSectionKey).BadgesInternal = Engine.GetBadgesDues(key).ToList();
            GetDashboardSections(key).Single(d => d.Text == TaskSectionKey).BadgesInternal = Engine.GetBadgesTasks(key, null).ToList();
        }


        static List<Dashboard> Dashboards = new List<Dashboard>();
        public static Dictionary<string, HashSet<string>> Groups = new Dictionary<string, HashSet<string>>();

        static List<TodoItem> Goals = new List<TodoItem>();
        //static List<TodoItem> Dues = new List<TodoItem>();
        static List<TodoItem> Tasks = new List<TodoItem>();

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
            foreach (var task in Tasks.Where(t => t.GroupKey == key && parentId == t.ParentId))
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

        public static IEnumerable<BadgeItem> GetBadgesDues(string key)
        {
            foreach (var task in Tasks.Where(t => t.GroupKey == key && !t.IsParent).OrderBy(t => t.Deadline == DateTimeOffset.MinValue ? DateTimeOffset.MaxValue : t.Deadline))
            {
                yield return new BadgeItem
                {
                    Id = task.Id,
                    Text = task.Text,
                    ParentId = task.ParentId,
                    LinkItems = GetLinkItems(task, key).ToList(),
                    Items = new List<BadgeItem>(),
                    Info = JsonSerializer.Serialize(task)
                };
            }
        }

        public static IEnumerable<BadgeItem> GetBadgesTasks(string key, string parentId)
        {
            foreach (var task in Tasks.Where(t => t.GroupKey == key && t.ParentId == parentId).Take(10))
            {
                yield return new BadgeItem
                {
                    Id = task.Id,
                    Text = task.Text,
                    ParentId = task.ParentId,
                    Status = task.Status,
                    LinkItems = GetLinkItems(task, key).ToList(),
                    Items = GetBadgesTasks(key, task.Id).ToList(),
                    Info = JsonSerializer.Serialize(task)
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
            var setDeadline = new
            {
                Action = "setDeadline",
                Key = key,
                Content = JsonSerializer.Serialize(new { Deadline = "[date]", Id = id })
            };
            var close = new
            {
                Action = "closeTask",
                Key = key,
                Content = JsonSerializer.Serialize(new { Id = id })
            };
            var start = new
            {
                Action = "startTask",
                Key = key,
                Content = JsonSerializer.Serialize(new { Id = id })
            };
            var pause = new
            {
                Action = "pauseTask",
                Key = key,
                Content = JsonSerializer.Serialize(new { Id = id })
            };


            yield return new LinkItem { Link = JsonSerializer.Serialize(addSteps), Text = "Steps" };
            yield return new LinkItem { Link = JsonSerializer.Serialize(delete), Text = "Delete" };
            yield return new LinkItem { Link = JsonSerializer.Serialize(update), Text = "Update" };
            yield return new LinkItem { Link = JsonSerializer.Serialize(setLocation), Text = "Location" };
            yield return new LinkItem { Link = JsonSerializer.Serialize(setTag), Text = "Tag" };
            yield return new LinkItem { Link = JsonSerializer.Serialize(setDeadline), Text = "Deadline" };
            if (!task.IsParent)
            {
                yield return new LinkItem { Link = JsonSerializer.Serialize(close), Text = "close" };
                if (task.Status == TodoStatus.start)
                {
                    yield return new LinkItem { Link = JsonSerializer.Serialize(pause), Text = "pause" };
                }
                else
                {
                    yield return new LinkItem { Link = JsonSerializer.Serialize(start), Text = "start" };
                }
            }
        }
    }

    #endregion
}


