using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

namespace PersonalAssistant
{
    public class DashboardPart
    {
        public string Text { get; set; }
        public string Description { get; set; }
        public List<BadgeItem> Badges => BadgesInternal;
        internal List<BadgeItem> BadgesInternal { set; get; } = new List<BadgeItem>();
        public int Sequence;
    }

    public class Dashboard
    {
        public Dashboard(string memberKey)
        {
            Id = memberKey;
            AssistantKey = memberKey;
            AddMemberSection();
        }

        void AddMemberSection()
        {
            Parts.Add(DashboardItemGoal());
            Parts.Add(DashboardItemTag());
            Parts.Add(DashboardItemLocation());
            Parts.Add(DashboardItemDue());
            Parts.Add(DashboardItemTask());
        }

        public string Id { get; set; }
        public string AssistantKey { get; set; }
        public string CurrentLocation { get; set; }
        public List<DashboardPart> Parts { get; } = new List<DashboardPart>();

        DashboardPart DashboardItemGoal()
            => new DashboardPart { Text = "Goal", Description = "Aim to do short or long term!", Sequence = 0, BadgesInternal = Engine.GetBadgesByGoal(AssistantKey, null).ToList() };

        static DashboardPart DashboardItemTag()
            => new DashboardPart { Text = "Tag", Description = "Tag should be able to get task or sort by selecting tag, e.g I am in shop now!", Sequence = 1 };

        static DashboardPart DashboardItemLocation()
            => new DashboardPart { Text = "UsedLocations", Description = "For now we manually select location until ...", Sequence = 2 };

        DashboardPart DashboardItemDue()
            => new DashboardPart { Text = "Due", Description = "Order by Due", Sequence = 3, BadgesInternal = Engine.GetBadgesDues(AssistantKey).ToList() };

        DashboardPart DashboardItemTask()
            => new DashboardPart { Text = "Task", Description = "All tasks", Sequence = 4, BadgesInternal = Engine.GetBadgesTasks(AssistantKey, null).ToList() };

    }

    public class BadgeItem
    {
        public string Id { get; set; }
        public string Text { get; set; }

        public string ParentId { get; set; }
        public List<LinkItem> LinkItems { get; set; }
        public BadgeType Type { get; set; }
        public int Count { get; set; }
        public List<BadgeItem> Items { get; set; } = new List<BadgeItem>();
        public string Info { get; set; }
    }

    public class LinkItem
    {
        public string Text { get; set; }
        public string Link { get; set; }
    }

    public enum BadgeType
    {
        Tag,
        Location
    }

    public class TodoItem
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string GroupKey { get; set; }
        public string Text { get; set; }
        public DateTimeOffset Deadline { get; set; }
        public bool IsParent { get; set; }

    }
}