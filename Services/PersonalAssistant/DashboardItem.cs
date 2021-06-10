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
        }

        public string Id { get; set; }
        public string AssistantKey { get; set; }
        public string CurrentLocation { get; set; }
        public List<DashboardPart> Parts { get; } = new List<DashboardPart>();

        DashboardPart DashboardItemGoal()
            => new DashboardPart { Text = "Goal", Description = "Aim to do short or long term!", Sequence = 0, BadgesInternal = Engine.GetBadgesByGoal(AssistantKey).ToList() };

        static DashboardPart DashboardItemTag()
            => new DashboardPart { Text = "Tag", Description = "Tag should be able to get task or sort by selecting tag, e.g I am in shop now!", Sequence = 1 };

        static DashboardPart DashboardItemLocation()
            => new DashboardPart { Text = "UsedLocations", Description = "For now we manually select location until ...", Sequence = 2 };

    }

    public class BadgeItem
    {
        public string Text { get; set; }
        public string Link { get; set; }
        public BadgeType Type { get; set; }
        public int Count { get; set; }
    }

    public enum BadgeType
    {
        Tag,
        Location
    }

    public class TodoItem
    {
        public string Id { get; set; }
        public string GroupKey { get; set; }
        public string Text { get; set; }
        public string Deadline { get; set; }
    }
}