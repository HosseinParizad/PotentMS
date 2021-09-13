using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PotentHelper
{
    public class Helper
    {
        public static dynamic GetMetadataByGroupKey(string groupKey)
            => GetMetadataByGroupKey(groupKey, Guid.NewGuid().ToString());

        public static dynamic GetMetadataByGroupKey(string groupKey, string id)
    => new { GroupKey = groupKey, ReferenceKey = id, CreateDate = DateTimeOffset.Now, Version = "V0.0" };

        public static dynamic Deserialize(string content)
            => JsonConvert.DeserializeObject<dynamic>(content
                , new IsoDateTimeConverter { DateTimeFormat = "yyyy/MM/dd" });


        public static T DeserializeObject<T>(string content)
            => JsonConvert.DeserializeObject<T>(content
                , new IsoDateTimeConverter { DateTimeFormat = "yyyy/MM/dd" });

    }

    // public class DisposableAction : IDisposable
    // {
    // 	public DisposableAction(Action dispose)
    // 	{
    // 		this.dispose = dispose;
    // 	}

    // 	public DisposableAction(Action initialise, Action dispose)
    // 		: this(dispose)
    // 	{
    // 		initialise?.Invoke();
    // 	}

    // 	readonly Action dispose;

    // 	public void Dispose()
    // 	{
    // 		dispose();
    // 	}
    // }


    public static class TestHelper
    {
        public static class BuildContent
        {
            public static class Task
            {
                #region MoveTask

                public static string MoveTask(string groupKey, string id, string newParentId)
                {
                    var content = new { Id = id, ToParentId = newParentId };
                    var msg = new Msg(action: MapAction.Task.MoveTask, metadata: Helper.GetMetadataByGroupKey(groupKey, id), content: content);
                    return msg.ToString();
                }

                #endregion

                #region UpdateTask

                public static string UpdateTask(string groupKey, string id, string newDescription)
                {
                    var content = new { Id = id, Description = newDescription };
                    var msg = new Msg(action: MapAction.Task.UpdateDescription, metadata: Helper.GetMetadataByGroupKey(groupKey, id), content: content);
                    return msg.ToString();
                }

                #endregion

                #region NewTask

                public static string NewTask(string groupKey, string description, string parentId) => NewTask(groupKey, Guid.NewGuid().ToString(), description, parentId);

                public static string NewTask(string groupKey, string id, string description, string parentId)
                {
                    var content = new { Description = description, ParentId = parentId };
                    var msg = new Msg(action: MapAction.Task.NewTask, metadata: Helper.GetMetadataByGroupKey(groupKey, id), content: content);
                    return msg.ToString();
                }

                #endregion

                #region DeleteTask

                public static string DeleteTask(string groupKey, object id)
                {
                    var content = new { Id = id };
                    var msg = new Msg(action: MapAction.Task.DelTask, metadata: Helper.GetMetadataByGroupKey(groupKey), content: content);
                    return msg.ToString();
                }

                #endregion
            }

            public static class Time
            {
                public static string StartTime(string groupKey, string parentId, string parentName, string memberKey)
                {
                    var content = new { ParentId = parentId, ParentName = parentName, MemberKey = memberKey };
                    var msg = new Msg(action: MapAction.Time.Start, metadata: Helper.GetMetadataByGroupKey(groupKey), content: content);
                    return msg.ToString();

                }
                public static string PauseTime(string groupKey, string parentId, string parentName, string memberKey)
                {
                    var content = new { ParentId = parentId, ParentName = parentName, MemberKey = memberKey };
                    var msg = new Msg(action: MapAction.Time.Pause, metadata: Helper.GetMetadataByGroupKey(groupKey), content: content);
                    return msg.ToString();
                }
                public static string DoneTime(string groupKey, string parentId, string parentName, string memberKey)
                {
                    var content = new { ParentId = parentId, ParentName = parentName, MemberKey = memberKey };
                    var msg = new Msg(action: MapAction.Time.Done, metadata: Helper.GetMetadataByGroupKey(groupKey), content: content);
                    return msg.ToString();
                }
            }

            public static class Memory
            {
                public static string NewMemory(string groupKey, string id, string text, string hint, string parentId)
                {
                    var content = new { Text = text, Hint = hint, ParentId = parentId };
                    var msg = new Msg(action: MapAction.Memory.NewMemory, metadata: Helper.GetMetadataByGroupKey(groupKey, id), content: content);
                    return msg.ToString();
                }

                public static string Learnt(string groupKey, string id)
                {
                    var content = new { Id = id };
                    var msg = new Msg(action: MapAction.Memory.LearntMemory, metadata: Helper.GetMetadataByGroupKey(groupKey), content: content);
                    return msg.ToString();
                }

                public static string Delete(string groupKey, string id)
                {
                    var content = new { Id = id };
                    var msg = new Msg(action: MapAction.Memory.DelMemory, metadata: Helper.GetMetadataByGroupKey(groupKey), content: content);
                    return msg.ToString();
                }
            }


            public static class Group
            {
                public static string AddMember(string groupKey, string newMember)
                {
                    var content = new { NewMember = newMember };
                    var msg = new Msg(action: MapAction.Group.NewMember, metadata: Helper.GetMetadataByGroupKey(groupKey), content: content);
                    return msg.ToString();
                }

                public static string NewGroup(string groupKey, string group)
                {
                    var content = new { Group = group };
                    var msg = new Msg(action: MapAction.Group.NewGroup, metadata: Helper.GetMetadataByGroupKey(groupKey), content: content);
                    return msg.ToString();

                }

                public static string UpdateGroup(string groupKey, string newGroupName)
                {
                    var content = new { NewGroupName = newGroupName };
                    var msg = new Msg(action: MapAction.Group.UpdateGroup, metadata: Helper.GetMetadataByGroupKey(groupKey), content: content);
                    return msg.ToString();
                }
                public static string DeleteGroup(string groupName)
                {
                    var content = new { GroupName = groupName };
                    var msg = new Msg(action: MapAction.Group.UpdateGroup, metadata: Helper.GetMetadataByGroupKey(groupName), content: content);
                    return msg.ToString();
                }
            }
        }
    }
}