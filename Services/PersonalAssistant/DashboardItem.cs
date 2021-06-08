using System;
using System.Linq;
using System.Collections.Generic;

namespace PersonalAssistant
{
    public class DashboardPart
    {
        public string Text { get; set; }
        public string Description { get; set; }
        public List<BadgeItem> Badges { get { return BadgesInternal; } }
        internal List<BadgeItem> BadgesInternal { set; get; } = new List<BadgeItem>();
        public int Sequence;
    }

    public class Dashboard
    {
        public Dashboard(string assistantKey, Dictionary<string, List<string>> groups)
        {
            Id = assistantKey;
            AssistantKey = assistantKey;
            Parts = new List<DashboardPart>();

            if (!groups.Any() || !groups.TryGetValue(assistantKey, out var value) || value.Count < 2)
            {
                AddMemberSection();
            }
            else
            {
                AddMemberSectionForGroup(groups[Id]);
            }
        }

        void AddMemberSection()
        {
            Parts.Add(DashboardItemGoal());
            Parts.Add(DashboardItemTag());
            Parts.Add(DashboardItemLocation());
        }

        void AddMemberSectionForGroup(IEnumerable<string> members)
        {
            foreach (var key in members)
            {
                Parts.Add(DashboardItemGroup(key));
            }
        }

        public string Id { get; set; }
        public string AssistantKey { get; set; }
        public string CurrentLocation { get; set; }
        public List<DashboardPart> Parts;

        static DashboardPart DashboardItemGoal()
            => new DashboardPart { Text = "Goal", Description = "Aim to do short or long term!", Sequence = 0, BadgesInternal = new List<BadgeItem> { new BadgeItem { Text = "Deadlines" } } };

        static DashboardPart DashboardItemTag()
            => new DashboardPart { Text = "Tag", Description = "Tag should be able to get task or sort by selecting tag, e.g i am in shop now!", Sequence = 1 };

        static DashboardPart DashboardItemLocation()
            => new DashboardPart { Text = "UsedLocations", Description = "For now we manually select location until ...", Sequence = 2 };

        static DashboardPart DashboardItemGroup(string key)
            => new DashboardPart { Text = key, Description = "Aim to do short or long term!", Sequence = 0, BadgesInternal = new List<BadgeItem>() };
    }

    public class BadgeItem
    {
        public string Text { get; set; }
        public string Link { get; set; }
        public BadgeType Type { get; set; }
    }

    public enum BadgeType
    {
        Tag,
        Location
    }
}