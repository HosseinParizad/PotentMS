using System.Collections.Generic;

namespace PersonalAssistant
{
    public class DashboardItem
    {
        public DashboardItem()
        {
            Badges = new List<string>();
        }
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public List<string> Badges { get; set; }
        public string AssistantKey { get; set; }
        public object Sequence { get; set; }
    }
}

