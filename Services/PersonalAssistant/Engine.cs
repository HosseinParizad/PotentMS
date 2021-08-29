using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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

                case FeedbackActions.TaskClosed:
                    ApplyCloseTask(feedback);
                    break;

                default:
                    break;
            }

            SendFeedbackMessage(FeedbackType.Info, "Info", DateTimeOffset.Parse(feedback.Metadata.CreateDate.ToString()), Helper.GetMetadataByGroupKey(feedback.Metadata.GroupKey.ToString()), "Feedback Processed in Personal Assistant!");
        }

        internal static void OnMemoryFeedback(Feedback feedback)
        {
            switch (feedback.Action)
            {
                case FeedbackActions.NewMemoryAdded:
                    ApplyNewMemoryAdded(feedback);
                    break;

                case FeedbackActions.MemoryDeleted:
                    ApplyMemoryDeleted(feedback);
                    break;

                default:
                    break;
            }

            SendFeedbackMessage(FeedbackType.Info, "Info", DateTimeOffset.Parse(feedback.Metadata.CreateDate.ToString()), Helper.GetMetadataByGroupKey(feedback.Metadata.GroupKey.ToString()), "Feedback Processed in Personal Assistant!");
        }

        static void SendFeedbackMessage(FeedbackType type, string action, DateTimeOffset actionTime, dynamic metadata, dynamic content)
        {
            if (Program.StartingTimeApp < actionTime)
            {

                ProducerHelper.SendAMessage(
                               MessageTopic.PersonalAssistantFeedback,
                               new Feedback(type: type, name: FeedbackGroupNames.PersonalAssistant, action: action, metadata: metadata, content: content)
                              )
                           .GetAwaiter().GetResult();
            }
        }

        static void ApplyNewGroupAdded(Feedback feedback)
        {
            var data = feedback.Content;
            var metadata = feedback.Metadata;
            var memberKey = data.MemberKey.ToString();
            var groupKey = data.GroupKey.ToString();

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
            var data = feedback.Content;
            var groupKey = feedback.Metadata.GroupKey.ToString();
            var id = data.Id.ToString();
            var goal = data.Goal.ToString();
            var item = new TodoItem { Text = goal, Id = id, GroupKey = groupKey };
            Goals.Add(item);
            OnGoalChanged(groupKey);
        }

        static void ApplyNewTaskAdded(Feedback feedback)
        {
            //var data = feedback.Content;
            //var groupKey = feedback.Metadata.GroupKey.ToString();
            //var id = data.Id.ToString();
            //var text = data.Text.ToString();

            //var parentId = data.ParentId.ToString();
            //parentId = parentId == "" ? null : parentId;
            //var item = new TodoItem { Text = text, Id = id, GroupKey = groupKey, ParentId = parentId };
            //Tasks.Add(item);
            //var parent = Tasks.SingleOrDefault(d => d.Id == parentId);
            //if (parent != null)
            //{
            //    parent.IsParent = true;
            //}
            //OnTaskChanged(groupKey);
            ApplyNewItemAdded(feedback, Tasks, OnTaskChanged, () => new TodoItem());
        }

        static void ApplyNewMemoryAdded(Feedback feedback)
        {
            var data = feedback.Content;
            var groupKey = feedback.Metadata.GroupKey.ToString();
            var id = data.Id.ToString();
            var text = data.Text.ToString();
            var hint = data.Hint.ToString();

            var parentId = data.ParentId.ToString();
            parentId = parentId == "" ? null : parentId;
            var item = new TodoItem();
            item.Text = text;
            item.Id = id;
            //item.Hint = hint;
            item.GroupKey = groupKey;
            item.ParentId = parentId;

            Memorizes.Add(item);
            var parent = Memorizes.SingleOrDefault(d => d.Id == parentId);
            if (parent != null)
            {
                parent.IsParent = true;
            }
            OnMemoryChanged(groupKey);

        }

        static void ApplyMemoryDeleted(Feedback feedback)
        {
            var data = feedback.Content;
            var id = data.Id.ToString();
            var groupKey = feedback.Metadata.GroupKey.ToString();
            var memory = Memorizes.SingleOrDefault(d => d.Id == id);
            if (memory != null)
            {
                Memorizes.Remove(memory);
                OnMemoryChanged(groupKey);
            }
        }

        static void ApplyNewItemAdded<T>(Feedback feedback, List<T> list, Action<string> OnListChange, Func<T> GetNewItem)
            where T : TodoItem
        {
            var data = feedback.Content;
            var groupKey = feedback.Metadata.GroupKey.ToString();
            var id = data.Id.ToString();
            var text = data.Text.ToString();

            var parentId = data.ParentId.ToString();
            parentId = parentId == "" ? null : parentId;
            var item = GetNewItem();
            item.Text = text;
            item.Id = id;
            item.GroupKey = groupKey;
            item.ParentId = parentId;

            list.Add(item);
            var parent = list.SingleOrDefault(d => d.Id == parentId);
            if (parent != null)
            {
                parent.IsParent = true;
            }
            OnListChange(groupKey);
        }

        static void ApplyUpdateTaskDescription(Feedback feedback)
        {
            var data = feedback.Content;
            var groupKey = feedback.Metadata.GroupKey.ToString();
            var id = data.Id.ToString();
            var text = data.Description.ToString();
            Tasks.Single(d => d.Id == id).Text = text;
            OnTaskChanged(groupKey);
        }


        static void ApplyTaskAssginedToMember(Feedback feedback)
        {
            var data = feedback.Content;
            //var groupKey = feedback.Metadata.GroupKey.ToString();
            var id = data.Id.ToString();
            var member = data.MemberKey.ToString();
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
            var newTag = feedback.Content.ToString();
            var groupKey = feedback.Metadata.GroupKey.ToString();
            List<BadgeItem> badges = GetDashboardSectionBadges(groupKey, "Tag");
            if (!badges.Any(b => b.Text == newTag))
            {
                badges.Add(new BadgeItem { Text = newTag });
            }
        }

        static void ApplyLocationAdded(Feedback feedback)
        {
            var data = feedback.Content;
            var groupKey = feedback.Metadata.GroupKey.ToString();
            var key = groupKey;
            var location = data.Location.ToString();
            List<BadgeItem> badges = GetDashboardSectionBadges(key, "UsedLocations");

            if (!badges.Any(b => b.Text == location))
            {
                badges.Add(new BadgeItem { Text = location, Type = BadgeType.Location });
                if (!Locations.ContainsKey(key))
                {
                    Locations.Add(key, new HashSet<string> { location });
                }
                else if (!Locations[key].Contains(location))
                {
                    Locations[key].Add(location);
                }
            }
            GetDashboardOrAdd(key).Locations.Add(location);
        }

        static void ApplyDeadlineUpdated(Feedback feedback)
        {
            var data = feedback.Content;
            var groupKey = feedback.Metadata.GroupKey.ToString();
            var id = data.Id.ToString();
            var deadline = data.Deadline;
            var task = Tasks.Single(t => t.Id == id);
            if (task != null)
            {
                task.Deadline = deadline;
            }
            OnTaskChanged(groupKey);
        }

        static void ApplyTaskDeleted(Feedback feedback)
        {
            var data = feedback.Content;
            var groupKey = feedback.Metadata.GroupKey.ToString();
            var id = data.Id.ToString();
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
                OnTaskChanged(groupKey);
            }
        }

        static void ApplyGoalDeleted(Feedback feedback)
        {
            var data = feedback.Content;
            var groupKey = feedback.Metadata.GroupKey.ToString();
            var id = data.Id.ToString();
            var goal = Goals.SingleOrDefault(t => t.Id == id);
            if (goal != null)
            {
                Goals.Remove(goal);
                GetDashboardSectionBadges(groupKey, GoalSectionKey).BadgesInternal.BadgesInternal = Engine.GetBadgesByGoal(groupKey, null).ToList();
            }
            var task = Tasks.SingleOrDefault(t => t.Id == id);
            if (task != null)
            {
                Tasks.Remove(goal);
                GetDashboardSectionBadges(groupKey, GoalSectionKey).BadgesInternal.BadgesInternal = Engine.GetBadgesTasks(groupKey, null).ToList();
            }
        }

        static void ApplyStartTask(Feedback feedback)
        {
            var data = feedback.Content;
            var groupKey = feedback.Metadata.GroupKey.ToString();
            var id = data.Id.ToString();
            Tasks.Single(d => d.Id == id).Status = TodoStatus.start;
            OnTaskChanged(groupKey);
        }

        static void ApplyPauseTask(Feedback feedback)
        {
            var data = feedback.Content;
            var groupKey = feedback.Metadata.GroupKey.ToString();
            var id = data.Id.ToString();
            Tasks.Single(d => d.Id == id).Status = TodoStatus.pause;
            OnTaskChanged(groupKey);
        }

        static void ApplyCloseTask(Feedback feedback)
        {
            var data = feedback.Content;
            var groupKey = feedback.Metadata.GroupKey.ToString();
            var id = data.Id.ToString();
            var task = Tasks.SingleOrDefault(d => d.Id == id);
            if (task != null)
            {
                task.Status = TodoStatus.closed;
                OnTaskChanged(groupKey);
            }
            else
            {
                var memoryItem = Memorizes.SingleOrDefault(d => d.Id == id);
                if (memoryItem != null)
                {
                    var nextdate = DateTimeOffset.Now.AddDays(1);
                    var stage = memoryItem.Stage;
                    switch (memoryItem.Stage)
                    {
                        case MemoryStage.Stage1:
                            nextdate = DateTimeOffset.Now.AddDays(1);
                            stage = MemoryStage.Stage2;
                            break;
                        case MemoryStage.Stage2:
                            nextdate = DateTimeOffset.Now.AddDays(3);
                            stage = MemoryStage.Stage3;
                            break;
                        case MemoryStage.Stage3:
                            nextdate = DateTimeOffset.Now.AddDays(7);
                            stage = MemoryStage.Stage4;
                            break;
                        case MemoryStage.Stage4:
                            nextdate = DateTimeOffset.Now.AddDays(14);
                            stage = MemoryStage.Stage5;
                            break;
                        case MemoryStage.Stage5:
                            nextdate = DateTimeOffset.Now.AddDays(30);
                            stage = MemoryStage.Stage6;
                            break;
                        case MemoryStage.Stage6:
                            nextdate = DateTimeOffset.MaxValue;
                            break;
                        default:
                            break;
                    }
                    memoryItem.NextMemorizeDate = nextdate;
                    memoryItem.Stage = stage;
                }
                OnMemoryChanged(groupKey);
            }
        }

        static List<BadgeItem> GetDashboardSectionBadges(string key, string sectionText)
            => GetDashboardSections(key).Single(d => d.Text == sectionText).BadgesInternal;

        static List<DashboardPart> GetDashboardSections(string key) => GetDashboardOrAdd(key).Parts;

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

        internal static void Reset(dynamic arg1, dynamic arg2)
        {
            Dashboards = new List<Dashboard>();
            Groups = new Dictionary<string, HashSet<string>>();
            Goals = new List<TodoItem>();
            Tasks = new List<TodoItem>();
            Memorizes = new List<TodoItem>();
            Locations = new Dictionary<string, HashSet<string>>();
            //Deadlines = new Dictionary<string, List<DeadlineItem>>();
        }

        #endregion

        #region Location actions 

        public static void SetCurrentLocation(dynamic groupKey, dynamic content)
        {
            var data = Helper.Deserialize(content);
            var key = data.Member.ToString();
            string location = data.Location.ToString();
            Dashboard dashbord = GetDashboard(key);
            dashbord.CurrentLocation = location;
        }

        #endregion

        //#region Location actions 

        //public static void SetUsedLocation(dynamic metadata, dynamic content)
        //{
        //    var data = JsonSerializer.Deserialize<dynamic>(content);
        //    var key = data.Member.ToString();
        //    string location = data.Location.ToString();
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
        const string OrderedSectionKey = "Ordered";
        const string MemorizesSectionKey = "Memorizes";

        static void OnTaskChanged(string key)
        {
            GetDashboardSections(key).Single(d => d.Text == DueSectionKey).BadgesInternal = Engine.GetBadgesDues(key).ToList();
            GetDashboardSections(key).Single(d => d.Text == TaskSectionKey).BadgesInternal = Engine.GetBadgesTasks(key, null).ToList();
            GetDashboardSections(key).Single(d => d.Text == OrderedSectionKey).BadgesInternal = Engine.GetBadgesOrdered(key, null).ToList();
            GetDashboardSections(key).Single(d => d.Text == GoalSectionKey).BadgesInternal = Engine.GetBadgesByGoal(key, null).ToList();
        }

        static void OnGoalChanged(string key)
        {
            GetDashboardSections(key).Single(d => d.Text == GoalSectionKey).BadgesInternal = Engine.GetBadgesByGoal(key, null).ToList();
        }

        static void OnMemoryChanged(string key)
        {
            GetDashboardSections(key).Single(d => d.Text == MemorizesSectionKey).BadgesInternal = Engine.GetBadgesMemorizes(key, null).ToList();
        }


        static List<Dashboard> Dashboards = new List<Dashboard>();
        public static Dictionary<string, HashSet<string>> Groups = new Dictionary<string, HashSet<string>>();

        static List<TodoItem> Goals = new List<TodoItem>();
        //static List<TodoItem> Dues = new List<TodoItem>();
        static List<TodoItem> Tasks = new List<TodoItem>();
        static List<TodoItem> Memorizes = new List<TodoItem>();
        public static Dictionary<string, HashSet<string>> Locations = new Dictionary<string, HashSet<string>>();

        public static IEnumerable<BadgeItem> GetBadgesByGoal(string key, string parentId)
        {
            var items = (parentId == null)
                   ? Goals.Where(t => t.GroupKey == key && t.ParentId == parentId)
                   : Tasks.Union(Goals).Where(t => t.GroupKey == key && t.ParentId == parentId);

            foreach (var task in items)
            {
                yield return new BadgeItem
                {
                    Id = task.Id,
                    Text = task.Text,
                    Status = task.Status,
                    ParentId = task.ParentId,
                    LinkItems = GetLinkItems(task, key, "Task").ToList(),
                    Items = GetBadgesDuesTree(key, task.Id).ToList(),
                    Info = JsonConvert.SerializeObject(task)
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
                    Status = task.Status,
                    ParentId = task.ParentId,
                    LinkItems = GetLinkItems(task, key, "Task").ToList(),
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
                    Status = task.Status,
                    LinkItems = GetLinkItems(task, key, "Task").ToList(),
                    Items = new List<BadgeItem>(),
                    Info = JsonConvert.SerializeObject(task)
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
                    LinkItems = GetLinkItems(task, key, "Task").ToList(),
                    Items = GetBadgesTasks(key, task.Id).ToList(),
                    Info = JsonConvert.SerializeObject(task)
                };
            }
        }

        public static IEnumerable<BadgeItem> GetBadgesOrdered(string key, string parentId)
        {
            foreach (var task in Tasks.Where(t => t.GroupKey == key && t.ParentId == parentId).Take(10))
            {
                yield return new BadgeItem
                {
                    Id = task.Id,
                    Text = task.Text,
                    ParentId = task.ParentId,
                    Status = task.Status,
                    LinkItems = GetLinkItems(task, key, "Task").ToList(),
                    Items = GetBadgesOrdered(key, task.Id).ToList(),
                    Info = JsonConvert.SerializeObject(task)
                };
            }
        }

        public static IEnumerable<BadgeItem> GetBadgesMemorizes(string key, string parentId)
        {
            //foreach (var task in Memorizes.Where(t => t.GroupKey == key && t.ParentId == parentId).OrderBy(t => t.Stage))
            foreach (var task in Memorizes.Where(t => t.GroupKey == key && t.ParentId == parentId && (t.IsParent || t.NextMemorizeDate <= DateTimeOffset.Now)).OrderBy(t => t.Stage))
            {
                yield return new BadgeItem
                {
                    Id = task.Id,
                    Text = $"{task.Text} ({task.Stage})",
                    ParentId = task.ParentId,
                    Status = task.Status,
                    Type = task.IsParent ? BadgeType.Catogory : BadgeType.None,
                    LinkItems = GetLinkItemsMemory(task, key, "Memory").ToList(),
                    Items = GetBadgesMemorizes(key, task.Id).ToList(),
                    Info = task.Hint
                };
            }
        }

        static IEnumerable<LinkItem> GetLinkItemsMemory(TodoItem task, string key, string kind, bool shortList = false)
        {
            var id = task.Id;
            Func<string, dynamic, dynamic> createStep = (action, content) =>
               new
               {
                   Group = "/Memory",
                   Action = action,
                   Metadata = new { GroupKey = key, ReferenceKey = Guid.NewGuid().ToString() },
                   Content = content
               };

            var addSteps = createStep("new" + kind, new { Description = "[text]", ParentId = id });
            var delete = createStep("del" + kind, new { Id = id });
            //var update = createStep("updateDescription", new { Description = "[text]", Id = id });
            //var setLocation = createStep("setLocation", new { Location = "[text]", Id = id });
            var setTag = createStep("setTag", new { Tag = "[text]", TagKey = 0, Id = id });
            //var setDeadline = createStep("setDeadline", new { Deadline = "[date]", Id = id });
            var learnt = createStep("closeTask", new { Id = id });
            //var start = createStep("startTask", new { Id = id });
            //var pause = createStep("pauseTask", new { Id = id });

            yield return new LinkItem { Link = JsonConvert.SerializeObject(addSteps), Text = "Steps" };
            yield return new LinkItem { Link = JsonConvert.SerializeObject(delete), Text = "Delete" };
            if (!shortList)
            {
                yield return new LinkItem { Link = JsonConvert.SerializeObject(setTag), Text = "Tag" };
                yield return new LinkItem { Link = $"{((MemoryItem)task).Hint}", Text = "link" };

                if (!task.IsParent)
                {
                    yield return new LinkItem { Link = JsonConvert.SerializeObject(learnt), Text = "learnt" };
                }
            }
        }

        static IEnumerable<LinkItem> GetLinkItems(TodoItem task, string key, string kind, bool shortList = false)
        {
            var id = task.Id;
            Func<string, dynamic, dynamic> createStep = (action, content) =>
               new
               {
                   Group = "",
                   Action = action,
                   Metadata = new { GroupKey = key, ReferenceKey = Guid.NewGuid().ToString() },
                   Content = content
               };

            var addSteps = createStep("new" + kind, new { Description = "[text]", ParentId = id });
            var delete = createStep("del" + kind, new { Id = id });
            var update = createStep("updateDescription", new { Description = "[text]", Id = id });
            var setLocation = createStep("setLocation", new { Location = "[text]", Id = id });
            var setTag = createStep("setTag", new { Tag = "[text]", TagKey = 0, Id = id });
            var setDeadline = createStep("setDeadline", new { Deadline = "[date]", Id = id });
            var close = createStep("closeTask", new { Id = id });
            var start = createStep("startTask", new { Id = id });
            var pause = createStep("pauseTask", new { Id = id });

            yield return new LinkItem { Link = JsonConvert.SerializeObject(addSteps), Text = "Steps" };
            yield return new LinkItem { Link = JsonConvert.SerializeObject(delete), Text = "Delete" };
            if (!shortList)
            {
                yield return new LinkItem { Link = JsonConvert.SerializeObject(update), Text = "Update" };
                yield return new LinkItem { Link = JsonConvert.SerializeObject(setTag), Text = "Tag" };
                if (kind == "Task")
                {
                    yield return new LinkItem { Link = JsonConvert.SerializeObject(setLocation), Text = "Location" };
                    yield return new LinkItem { Link = JsonConvert.SerializeObject(setDeadline), Text = "Deadline" };
                }
                if (kind == "Memory")
                {
                    yield return new LinkItem { Link = $"{((MemoryItem)task).Hint}", Text = "link" };
                }

                if (!task.IsParent)
                {
                    yield return new LinkItem { Link = JsonConvert.SerializeObject(close), Text = (kind == "Task" ? "close" : "learnt") };
                    if (task.Status == TodoStatus.start)
                    {
                        yield return new LinkItem { Link = JsonConvert.SerializeObject(pause), Text = "pause" };
                    }
                    else
                    {
                        yield return new LinkItem { Link = JsonConvert.SerializeObject(start), Text = "start" };
                    }
                }
            }
        }
    }

    #endregion
}


