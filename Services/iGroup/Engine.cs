using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

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
                SendFeedbackMessage(type: FeedbackType.Success, actionTime: GetCreateDate(metadata), action: FeedbackActions.NewGroupAdded, groupkey: groupName, content: group);
            }
            else
            {
                SendFeedbackMessage(type: FeedbackType.Error, actionTime: GetCreateDate(metadata), action: FeedbackActions.CannotAddGroup, groupkey: metadata.GroupKey.ToString(), content: "Cannot add dupicate Group item!");
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
            var groupkey = metadata.GroupKey.ToString();
            var groupItems = Groups.Where(t => t.GroupKey == groupkey);
            if (groupItems.Any() && !Groups.Where(t => t.Id == newName).Any())
            {
                var mGroup = Groups.Single(t => t.GroupKey == groupkey && t.MemberKey == groupkey);
                mGroup.GroupKey = newName;
                mGroup.MemberKey = newName;
                foreach (var group in groupItems)
                {
                    group.GroupKey = newName;
                }
            }
            else
            {
                SendFeedbackMessage(type: FeedbackType.Error, actionTime: GetCreateDate(metadata), action: FeedbackActions.CannotUpdateGroup, groupkey: metadata.GroupKey.ToString(), content: "Cannot update Group");
            }

        }

        #endregion


        #region AddMember

        public static void AddMember(dynamic metadata, dynamic content)
        {
            var newMember = content.NewMember.ToString();
            var groupkey = metadata.GroupKey.ToString();
            var id = metadata.ReferenceKey.ToString();
            var groupItems = Groups.Where(t => t.GroupKey == groupkey);
            if (!groupItems.Any(g => g.MemberKey == newMember))
            {
                var group = new GroupItem { Id = id, GroupKey = groupkey, MemberKey = newMember };
                Groups.Add(group);
                SendFeedbackMessage(type: FeedbackType.Success, actionTime: GetCreateDate(metadata), action: FeedbackActions.NewMemberAdded, groupkey: groupkey, content: group);
            }
            else
            {
                SendFeedbackMessage(type: FeedbackType.Error, actionTime: GetCreateDate(metadata), action: FeedbackActions.CannotAddMember, groupkey: metadata.GroupKey.ToString(), content: "Cannot add member");
            }

        }

        #endregion

        #region DeleteGroup

        internal static void DeleteGroup(dynamic metadata, dynamic content)
        {
            var groupName = content.GroupName.ToString();
            var groupItems = Groups.Where(t => t.GroupKey == groupName);
            var groupkey = metadata.GroupKey.ToString();
            if (groupName == groupkey && groupItems.Any())
            {
                foreach (var group in groupItems)
                {
                    Groups.Remove(group);
                }
                SendFeedbackMessage(type: FeedbackType.Success, actionTime: GetCreateDate(metadata), action: FeedbackActions.GroupDeleted, groupkey: metadata.GroupKey.ToString(), content: "Group has been deleted");
            }
            else
            {
                SendFeedbackMessage(type: FeedbackType.Error, actionTime: GetCreateDate(metadata), action: FeedbackActions.CannotFindGroup, groupkey: metadata.GroupKey.ToString(), content: "Cannot find delete item!");
            }
        }

        #endregion

        #region DeleteMember

        internal static void DeleteMember(dynamic metadata, dynamic content)
        {
            var newMember = content.NewMember.ToString();
            var groupkey = metadata.GroupKey.ToString();
            var member = Groups.SingleOrDefault(t => t.GroupKey == groupkey && t.MemberKey == newMember);
            if (member != null)
            {
                Groups.Remove(member);
                SendFeedbackMessage(type: FeedbackType.Success, actionTime: GetCreateDate(metadata), action: FeedbackActions.GroupDeleted, groupkey: metadata.GroupKey.ToString(), content: "Memeber has been deleted");
            }
            else
            {
                SendFeedbackMessage(type: FeedbackType.Error, actionTime: GetCreateDate(metadata), action: FeedbackActions.CannotFindMember, groupkey: metadata.GroupKey.ToString(), content: "Cannot find member!");
            }
        }

        #endregion

        internal static IEnumerable<PresentItem> GetGroupPresentation(string groupKey)
        {
            return Groups.Where(i => (i.GroupKey == groupKey || groupKey == "All")).Select(i => GroupToPresentation(groupKey, i));
        }

        static PresentItem GroupToPresentation(string groupKey, GroupItem mi)
        {
            var presentItem = new PresentItem
            {
                Id = mi.Id,
                Text = mi.MemberKey,
                Link = "",
                Actions = GetActions(mi).ToList(),
                Items = new()
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
                   Metadata = new { GroupKey = mi.GroupKey, ReferenceKey = Guid.NewGuid().ToString() },
                   Content = content
               };
            yield return createStep("Add group", MapAction.Group.NewGroup, new { Group = "[text]" });
            yield return createStep("update", MapAction.Group.UpdateGroup, new { NewGroupName = "[text]" });
            yield return createStep("Add member", MapAction.Group.NewMember, new { NewMember = "[text]" });
            yield return createStep("delete group :(", MapAction.Group.DeleteGroup, new { GroupName = "[text]" });
            yield return createStep("remove member :(", MapAction.Group.DeleteMember, new { NewMember = "[text]" });
        }

        #region Implement

        static void SendFeedbackMessage(FeedbackType type, string action, DateTimeOffset actionTime, string groupkey, dynamic content)
        {
            if (Program.StartingTimeApp < actionTime)
            {
                ProducerHelper.SendAMessage(
                        MessageTopic.GroupFeedback,
                        new Feedback(type: type, name: FeedbackGroupNames.Group, action: action, metadata: Helper.GetMetadataByGroupKey(groupkey), content: content)
                        )
                    .GetAwaiter().GetResult();
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
