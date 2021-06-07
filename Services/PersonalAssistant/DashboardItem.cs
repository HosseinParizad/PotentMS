using System;
using System.Collections.Generic;

namespace PersonalAssistant
{
    public class TodosItem
    {
        public TodosItem()
        {
            Badges = new List<string>();
        }
        public string Id { get; set; }
        public string AssistantKey { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public List<string> Badges { get; set; }
        public List<string> UsedLocations { get; set; }
        public object Sequence { get; set; }
        public string CurrentLocation { set; get; }
    }

    public class DashboardPart
    {
        public string Text { get; set; }
        public string Description { get; set; }
        public List<BadgeItem> Badges { set; get; } = new List<BadgeItem>();
        public int Sequence;
    }

    public class Dashboard
    {
        public Dashboard(string assistantKey)
        {
            Id = assistantKey;
            AssistantKey = assistantKey;
            Parts = new List<DashboardPart>();
            Parts.Add(DashboardItemGoal());
            Parts.Add(DashboardItemTag());
            Parts.Add(DashboardItemLocation());
        }

        public string Id { get; set; }
        public string AssistantKey { get; set; }
        public string CurrentLocation { get; set; }
        public List<DashboardPart> Parts;

        static DashboardPart DashboardItemGoal()
            => new DashboardPart { Text = "Goal", Description = "Aim to do short or long term!", Sequence = 0, Badges = new List<BadgeItem> { new BadgeItem { Text = "Deadlines" } } };

        static DashboardPart DashboardItemTag()
            => new DashboardPart { Text = "Tag", Description = "Tag should be able to get task or sort by selecting tag, e.g i am in shop now!", Sequence = 1 };

        static DashboardPart DashboardItemLocation()
            => new DashboardPart { Text = "UsedLocations", Description = "For now we manually select location until ...", Sequence = 2 };
    }

    public class BadgeItem
    {
        public string Text { get; set; }
        public string Link { get; set; }
    }
}

