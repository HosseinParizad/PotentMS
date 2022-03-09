using System.Collections.Generic;

namespace PotentHelper
{
    public class PresentItem
    {
        public string Id { get; set; }
        public PresentItemType Type { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public List<PresentItemActions> Actions { get; set; }
        public List<PresentItem> Items { get; set; } = new List<PresentItem>(); 
        public string Info { get; set; }

    }
    public enum PresentItemType
    {
        Category,
        Node
    }
    public class PresentItemActions
    {
        public string Text { get; set; }
        public string Group { get; set; }
        public string Action { get; set; }
        public dynamic Metadata { get; set; }
        public dynamic Content { get; set; }
    }
}
