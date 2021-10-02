using PotentHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iGroup
{
    public class Engine
    {

        #region CreateNewGroup

        public static void CreateNewGroup(dynamic metadata, dynamic content)
        {
            var groupName = content.Group.ToString();
            var id = metadata.ReferenceKey.ToString();
            if (!Groups.Any(t => t.GroupKey == groupName))
            {
                var group = new GroupItem { Id = id, GroupKey = groupName, MemberKey = groupName };
                Groups.Add(group);
                SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: MapAction.GroupFeedback.NewGroupAdded.Name, content: group);
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: MapAction.GroupFeedback.CannotAddGroup.Name, content: "Cannot add dupicate Group item!");
            }
        }

        internal static IEnumerable<GroupItem> GetGroupsTestOnly()
        {
            return Groups;
        }

        #endregion

        #region UpdateGroup

        public static void UpdateGroup(dynamic metadata, dynamic content)
        {
            var newName = content.NewGroupName.ToString();
            var groupKey = metadata.GroupKey.ToString();
            var groupItems = Groups.Where(t => t.GroupKey == groupKey);
            if (groupItems.Any() && !Groups.Where(t => t.Id == newName).Any())
            {
                var mGroup = Groups.Single(t => t.GroupKey == groupKey && t.MemberKey == groupKey);
                mGroup.GroupKey = newName;
                mGroup.MemberKey = newName;
                foreach (var group in groupItems)
                {
                    group.GroupKey = newName;
                }
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: MapAction.GroupFeedback.CannotUpdateGroup.Name, content: "Cannot update Group");
            }

        }

        #endregion


        #region AddMember

        public static void AddMember(dynamic metadata, dynamic content)
        {
            var newMember = content.NewMember.ToString();
            var groupKey = metadata.GroupKey.ToString();
            var id = metadata.ReferenceKey.ToString();
            var groupItems = Groups.Where(t => t.GroupKey == groupKey);
            if (!groupItems.Any(g => g.MemberKey == newMember))
            {
                var group = new GroupItem { Id = id, GroupKey = groupKey, MemberKey = newMember };
                Groups.Add(group);
                SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: MapAction.GroupFeedback.NewMemberAdded.Name, content: group);
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: MapAction.GroupFeedback.CannotAddMember.Name, content: "Cannot add member");
            }

        }

        #endregion

        #region DeleteGroup

        internal static void DeleteGroup(dynamic metadata, dynamic content)
        {
            var groupName = content.GroupName.ToString();
            var groupItems = Groups.Where(t => t.GroupKey == groupName);
            var groupKey = metadata.GroupKey.ToString();
            if (groupName == groupKey && groupItems.Any())
            {
                foreach (var group in groupItems)
                {
                    Groups.Remove(group);
                    SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: MapAction.GroupFeedback.GroupDeleted.Name, content: new { GroupKey = group.GroupKey, MemberKey = group.MemberKey });
                }
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: MapAction.GroupFeedback.CannotFindGroup.Name, content: "Cannot find delete item!");
            }
        }

        #endregion

        #region DeleteMember

        internal static void DeleteMember(dynamic metadata, dynamic content)
        {
            var member = content.Member.ToString();
            var groupKey = metadata.GroupKey.ToString();
            var memberFound = Groups.SingleOrDefault(t => t.GroupKey == groupKey && t.MemberKey == member);
            if (memberFound != null)
            {
                Groups.Remove(member);
                SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: MapAction.GroupFeedback.GroupDeleted.Name, content: new { GroupKey = member.GroupKey, MemberKey = member.MemberKey });
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: MapAction.GroupFeedback.CannotFindMember.Name, content: "Cannot find member!");
            }
        }

        #endregion

        internal static IEnumerable<PresentItem> GetGroupPresentation(string groupKey, string memberKey)
        {
            return Groups.Where(i => (i.GroupKey == groupKey && (memberKey == null || i.MemberKey == memberKey)) || ((groupKey == null || i.GroupKey == groupKey) && i.MemberKey == memberKey)).Select(i => GroupToPresentation(groupKey, memberKey, i));
        }

        static PresentItem GroupToPresentation(string groupKey, string memberKey, GroupItem mi)
        {
            var presentItem = new PresentItem
            {
                Id = mi.Id,
                Text = mi.MemberKey,
                Link = "",
                Actions = GetActions(mi).ToList(),
                Items = new(),
                Info = memberKey
            };
            return presentItem;
        }

        static IEnumerable<PresentItemActions> GetActions(GroupItem mi)
        {
            Func<string, string, dynamic, PresentItemActions> createStep = (text, action, content) =>
               new PresentItemActions
               {
                   Text = text,
                   Group = "/Group",
                   Action = action,
                   Metadata = new { GroupKey = mi.GroupKey, MemberKey = mi.MemberKey, ReferenceKey = Guid.NewGuid().ToString() },
                   Content = content
               };
            yield return createStep("Add group", MapAction.Group.NewGroup.Name, new { Group = "[text]" });
            yield return createStep("update", MapAction.Group.UpdateGroup.Name, new { NewGroupName = "[text]" });
            yield return createStep("Add member", MapAction.Group.NewMember.Name, new { NewMember = "[text]" });
            yield return createStep("delete group :(", MapAction.Group.DeleteGroup.Name, new { GroupName = "[text]" });
            yield return createStep("remove member :(", MapAction.Group.DeleteMember.Name, new { NewMember = "[text]" });
        }

        #region Implement

        static void SendFeedbackMessage(MsgType type, string action, DateTimeOffset actionTime, dynamic content)
        {
            if (Program.StartingTimeApp < actionTime)
            {
                ProducerHelper.SendMessage(MessageTopic.GroupFeedback, new Feedback(type: type, action: action, content: content)).GetAwaiter().GetResult();
            }
        }

        static DateTimeOffset GetCreateDate(dynamic metadata)
        {
            return DateTimeOffset.Parse(metadata.CreateDate.ToString());
        }

        #endregion

        internal static IEnumerable<GroupItem> GetGroup(string groupKey)
        {
            return Groups.Where(m => m.GroupKey == groupKey);
        }

        #region Common actions

        public static void Reset(dynamic metadata, dynamic content)
        {
            Groups = new List<GroupItem>();
        }

        public static List<GroupItem> Groups { get; private set; } = new List<GroupItem>();

        #endregion

        #region Mapping

        public static List<MapBinding> Mapping = new()
        {
            new MapBinding(MapAction.Common.Reset, Engine.Reset),
            new MapBinding(MapAction.Group.NewGroup, Engine.CreateNewGroup),
            new MapBinding(MapAction.Group.UpdateGroup, Engine.UpdateGroup),
            new MapBinding(MapAction.Group.NewMember, Engine.AddMember),
            new MapBinding(MapAction.Group.DeleteMember, Engine.DeleteMember),
        };

        public static string AppId => KafkaEnviroment.preFix + "iGroup";

        #endregion
    }
    public class GroupItem
    {
        public string Id { get; internal set; }
        public string MemberKey { get; internal set; }
        public string GroupKey { get; internal set; }
        public GroupType GroupType => GroupKey == MemberKey ? GroupType.Group : GroupType.Member;
    }

    public enum GroupType
    {
        Group,
        Member
    }
}
