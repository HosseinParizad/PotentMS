using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;

namespace iAssistant
{
    public class Engine
    {
        internal static IEnumerable<PresentItem> GetPresentation(string groupKey, string memberKey, string parentid)
        {
            var sections = new[] { "Goals", "Todos", "Dues", "By Location", "By Tag", "Memorizes" };
            return sections.Select(s => ToPresentation(s));
            //return Todos.Where(i => i.GroupKey == groupKey && i.ParentId == parentid).OrderBy(t => t.Location == MemberLocation[t.GroupKey] ? 0 : 1).Select(i => ToPresentation(groupKey, i));
        }

        static PresentItem ToPresentation(string section)
        {
            var presentItem = new PresentItem
            {
                Id = section,
                Text = section,
                Link = ""
                //Actions = GetActions(mi).ToList(),
                //Items = GetPresentation(groupKey, mi.Id).ToList()
            };
            return presentItem;
        }

        internal static void OnNewTaskAdded(dynamic metadata, dynamic content)
        {
            var groupKey = metadata.GroupKey.ToString();
            var memberKey = metadata.MemberKey.ToString();

            Todos.Add(new TodoItem
            {
                Id = content.Id.ToString(),
                Text = content.Text.ToString(),
                ParentId = content.ParentId.ToString(),
                GroupKey = groupKey,
                MemberKey = memberKey
            });

            if (!MemberLocation.ContainsKey(groupKey))
            {
                MemberLocation.Add(groupKey, "");
            }
        }

        internal static void MemberSetLocation(dynamic metadata, dynamic content)
        {
            var groupKey = metadata.GroupKey.ToString();
            var memberKey = metadata.MemberKey.ToString();
            var location = content.Location.ToString();
            var id = content.Id.ToString();

            var todo = Todos.SingleOrDefault(t => t.Id == id);
            if (todo != null)
            {
                todo.Location = location;
            }
        }

        static Dictionary<string, string> MemberLocation = new();

        internal static void MemberMoved(dynamic metadata, dynamic content)
        {
            var groupKey = metadata.GroupKey.ToString();
            var memberKey = metadata.MemberKey.ToString();
            var newLocation = content.NewLocation.ToString();
            if (MemberLocation.ContainsKey(groupKey))
            {
                MemberLocation[groupKey] = newLocation;
            }
            else
            {
                MemberLocation.Add(groupKey, newLocation);
            }
        }

        #region Implement

        static DateTimeOffset GetCreateDate(dynamic metadata)
        {
            return DateTimeOffset.Parse(metadata.CreateDate.ToString());
        }

        #endregion

        #region Common actions

        public static void Reset(dynamic metadata, dynamic content)
        {
            Todos = new();
        }

        static List<TodoItem> Todos = new();

        #endregion
    }

    public class TodoItem
    {
        public string Id { get; internal set; }
        public string Text { get; internal set; }
        public string GroupKey { get; internal set; }
        public string MemberKey { get; internal set; }
        public string ParentId { get; internal set; }
        public string Location { get; internal set; }
    }

}
