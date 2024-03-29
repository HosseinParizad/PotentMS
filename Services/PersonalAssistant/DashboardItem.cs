﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalAssistant
{
    public class Dashboard
    {
        #region ctor

        public Dashboard(string memberKey)
        {
            Text = memberKey;
            AssistantKey = memberKey;
            AddMemberSection();
        }

        #endregion

        #region prop

        public string Text { get; set; }
        public string AssistantKey { get; set; }
        public HashSet<string> Locations { get; set; } = new HashSet<string>();
        public string CurrentLocation { get; set; }
        public List<DashboardPart> Parts { get; } = new List<DashboardPart>();

        #endregion

        #region DashboardPart

        void AddMemberSection()
        {
            Parts.Add(DashboardItemGoal());
            Parts.Add(DashboardItemTag());
            Parts.Add(DashboardItemLocation());
            Parts.Add(DashboardItemDue());
            Parts.Add(DashboardItemTask());
            Parts.Add(DashboardItemOrdered());
            Parts.Add(DashboardItemMemory());
        }

        DashboardPart DashboardItemGoal()
            => new DashboardPart { Text = "Goal", Description = "Aim to do short or long term!", Sequence = 0 };

        static DashboardPart DashboardItemTag()
                => new DashboardPart { Text = "Tag", Description = "Tag should be able to get task or sort by selecting tag, e.g I am in shop now!", Sequence = 1 };

        static DashboardPart DashboardItemLocation()
            => new DashboardPart { Text = "UsedLocations", Description = "For now we manually select location until ...", Sequence = 2 };

        DashboardPart DashboardItemDue()
            => new DashboardPart { Text = "Due", Description = "Order by Due", Sequence = 3, BadgesInternal = Engine.GetBadgesDues(AssistantKey).ToList() };

        DashboardPart DashboardItemTask()
            => new DashboardPart { Text = "Task", Description = "All tasks", Sequence = 4 };

        DashboardPart DashboardItemOrdered()
            => new DashboardPart { Text = "Ordered", Description = "Ordered", Sequence = 5, BadgesInternal = Engine.GetBadgesOrdered(AssistantKey, null).ToList() };

        DashboardPart DashboardItemMemory()
            => new DashboardPart { Text = "Memorizes", Description = "Memorizes", Sequence = 6 };

        #endregion
    }

    #region Releated Classes

    public class DashboardPart
    {
        public string Text { get; set; }
        public string Description { get; set; }
        public List<BadgeItem> Badges => BadgesInternal;
        internal List<BadgeItem> BadgesInternal { set; get; } = new List<BadgeItem>();
        public int Sequence;
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
        public TodoStatus Status { get; set; }
        public string LinkGetData { get; set; }

    }

    public class LinkItem
    {
        public string Text { get; set; }
        public string Link { get; set; }
    }

    public enum BadgeType
    {
        None,
        Tag,
        Location,
        Catogory
    }

    public class TodoItem
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string GroupKey { get; set; }
        public string Text { get; set; }
        public DateTimeOffset Deadline { get; set; }
        public bool IsParent { get; set; }
        public TodoStatus Status { get; set; }
    }

    public enum TodoStatus
    {
        none,
        start,
        pause,
        closed
    }

    #endregion
}