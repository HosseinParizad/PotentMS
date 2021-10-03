using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PotentHelper
{
    public static class Helper
    {
        public static dynamic GetMetadataByGroupKey(string groupKey, string memberKey)
            => GetMetadataByGroupKey(groupKey, memberKey, Guid.NewGuid().ToString());

        public static dynamic GetMetadataByGroupKey(string groupKey, string memberKey, string id)
            => new { GroupKey = groupKey, MemberKey = memberKey, ReferenceKey = id, CreateDate = DateTimeOffset.Now, Version = "V0.0" };

        public static dynamic Deserialize(string content)
            => JsonConvert.DeserializeObject<dynamic>(content
                , new IsoDateTimeConverter { DateTimeFormat = "yyyy/MM/dd" });


        public static T DeserializeObject<T>(string content)
            => JsonConvert.DeserializeObject<T>(content
                , new IsoDateTimeConverter { DateTimeFormat = "yyyy/MM/dd" });

        public static IEnumerable<TSource> GetGroupMember<TSource>(this IEnumerable<TSource> source, string groupKey, string memberKey)
            => source.Cast<IMultiGroup>().Where(mg => ((mg.GroupKey == groupKey && (memberKey == null || memberKey == "" || mg.MemberKey == memberKey)) || ((groupKey == null || groupKey == "" || mg.GroupKey == groupKey) && mg.MemberKey == memberKey))).Cast<TSource>();

        public static IEnumerable<TSource> GetGroupMember<TSource>(this IEnumerable<TSource> source, string groupKey, string memberKey, string parentId)
            => source.Cast<IMultiGroupParent>().GetGroupMember(groupKey, memberKey).Where(mg => mg.ParentId == parentId).Cast<TSource>();
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

                public static string MoveTask(string groupKey, string memberKey, string id, string newParentId)
                {
                    var content = new { Id = id, ToParentId = newParentId };
                    var msg = new Msg(action: MapAction.Task.MoveTask, metadata: Helper.GetMetadataByGroupKey(groupKey, memberKey, id), content: content);
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

                public static string NewTask(string groupKey, string memberKey, string description, string parentId) => NewTask(groupKey, memberKey, Guid.NewGuid().ToString(), description, parentId);

                public static string NewTask(string groupKey, string memberKey, string id, string description, string parentId)
                {
                    var content = new { Description = description, ParentId = parentId };
                    var msg = new Msg(action: MapAction.Task.NewTask, metadata: Helper.GetMetadataByGroupKey(groupKey, memberKey, id), content: content);
                    return msg.ToString();
                }

                #endregion

                #region DeleteTask

                public static string DeleteTask(string groupKey, string memberKey, string id)
                {
                    var content = new { Id = id };
                    var msg = new Msg(action: MapAction.Task.DelTask, metadata: Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);
                    return msg.ToString();
                }

                #endregion

                #region SetLocation

                public static string SetLocation(string groupKey, string memberKey, string id, string location)
                {
                    var content = new { Id = id, Location = location };
                    var msg = new Msg(action: MapAction.Task.SetLocation, Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);
                    return msg.ToString();
                }

                #endregion

                #region SetTag

                public static string SetTag(string groupKey, string memberKey, string id, string tagKey, string tag)
                {
                    var content = new { Id = id, TagKey = tagKey, Tag = tag };
                    var msg = new Msg(action: MapAction.Task.SetTag, Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);
                    return msg.ToString();
                }

                #endregion
            }

            public static class Time
            {
                public static string StartTime(string groupKey, string memberKey, string parentId, string parentName)
                {
                    var content = new { ParentId = parentId, ParentName = parentName, MemberKey = memberKey };
                    var msg = new Msg(action: MapAction.Time.Start, Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);
                    return msg.ToString();

                }
                public static string PauseTime(string groupKey, string memberKey, string parentId, string parentName)
                {
                    var content = new { ParentId = parentId, ParentName = parentName, MemberKey = memberKey };
                    var msg = new Msg(action: MapAction.Time.Pause, Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);
                    return msg.ToString();
                }
                public static string DoneTime(string groupKey, string memberKey, string parentId, string parentName)
                {
                    var content = new { ParentId = parentId, ParentName = parentName, MemberKey = memberKey };
                    var msg = new Msg(action: MapAction.Time.Done, Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);
                    return msg.ToString();
                }
            }

            public static class Memory
            {
                public static string NewMemory(string groupKey, string memberKey, string id, string text, string hint, string parentId)
                {
                    var content = new { Text = text, Hint = hint, ParentId = parentId };
                    var msg = new Msg(action: MapAction.Memory.NewMemory, metadata: Helper.GetMetadataByGroupKey(groupKey, memberKey, id), content: content);
                    return msg.ToString();
                }

                public static string Learnt(string groupKey, string memberKey, string id)
                {
                    var content = new { Id = id };
                    var msg = new Msg(action: MapAction.Memory.LearntMemory, Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);
                    return msg.ToString();
                }

                public static string Delete(string groupKey, string memberKey, string id)
                {
                    var content = new { Id = id };
                    var msg = new Msg(action: MapAction.Memory.DelMemory, Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);
                    return msg.ToString();
                }
            }

            public static class Group
            {
                public static string AddMember(string groupKey, string newMember)
                {
                    var content = new { NewMember = newMember };
                    var msg = new Msg(action: MapAction.Group.NewMember, Helper.GetMetadataByGroupKey(groupKey, ""), content: content);
                    return msg.ToString();
                }

                public static string NewGroup(string groupKey, string group)
                {
                    var content = new { Group = group };
                    var msg = new Msg(action: MapAction.Group.NewGroup, Helper.GetMetadataByGroupKey(groupKey, ""), content: content);
                    return msg.ToString();

                }

                public static string UpdateGroup(string groupKey, string newGroupName)
                {
                    var content = new { NewGroupName = newGroupName };
                    var msg = new Msg(action: MapAction.Group.UpdateGroup, Helper.GetMetadataByGroupKey(groupKey, ""), content: content);
                    return msg.ToString();
                }
                public static string DeleteGroup(string groupName)
                {
                    var content = new { GroupName = groupName };
                    var msg = new Msg(action: MapAction.Group.UpdateGroup, metadata: Helper.GetMetadataByGroupKey(groupName, ""), content: content);
                    return msg.ToString();
                }
            }

            public static class Location
            {
                public static string RegisterMember(string groupKey, string member)
                {
                    var content = new { Member = member };
                    var msg = new Msg(action: MapAction.Location.RegisterMember, metadata: Helper.GetMetadataByGroupKey(groupKey, member), content: content);
                    return msg.ToString();
                }

                public static string TestOnlyLocationChanged(string memberKey, string location)
                {
                    var content = new { Location = location };
                    var msg = new Msg(action: MapAction.Location.TestOnlyLocationChanged, metadata: Helper.GetMetadataByGroupKey(memberKey, memberKey), content: content);
                    return msg.ToString();
                }
            }
        }
    }

    public abstract class SetupActionsCore
    {
        public DbText db = new();
        public string AppId => KafkaEnviroment.preFix + AppGroupId;

        public abstract List<MapBinding> Mapping { get; }

        public abstract string AppGroupId { get; }

        public async void Ini(bool replayAll = true)
        {
            db.Initial(AppId + "DB.txt");
            db.OnDbNewDataEvent += Db_DbNewDataEvent;

            if (replayAll)
            {
                db.ReplayAll();
            }

            MapAsync();
        }

        public void Db_DbNewDataEvent(object sender, DbNewDataEventArgs e)
        {
            MessageProcessor.MapMessageToAction(AppId, e.Text, Mapping);
        }

        void MapAsync() => ConsumerHelper.MapTopicToMethod(Mapping, db, AppId);

    }
    public interface IMultiGroupParent : IMultiGroup
    {
        string ParentId { get; }
    }

    public interface IMultiGroup
    {
        string GroupKey { get; }
        string MemberKey { get; }
    }
}