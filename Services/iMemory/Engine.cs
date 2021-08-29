using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace iMemory
{
    public class Engine
    {

        #region CreateNewMemory

        public static void CreateNewMemory(dynamic metadata, dynamic content)
        {
            NewMemoryItem(metadata, content, MemoryType.Memory);
        }

        #endregion

        #region CreateMemoryCategory

        public static void CreateMemoryCategory(dynamic metadata, dynamic content)
        {
            NewMemoryItem(metadata, content, MemoryType.Category);
        }

        #endregion

        #region DeleteMemory

        internal static void DeleteMemory(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var memory = Memories.SingleOrDefault(t => t.Id == id);
            var groupkey = metadata.GroupKey.ToString();
            if (memory != null)
            {
                Memories.Remove(memory);
                SendFeedbackMessage(type: FeedbackType.Success, actionTime: GetCreateDate(metadata), action: FeedbackActions.MemoryDeleted, groupkey: metadata.GroupKey.ToString(), content: new { Id = id });
            }
            else
            {
                SendFeedbackMessage(type: FeedbackType.Error, actionTime: GetCreateDate(metadata), action: FeedbackActions.CannotFindMemory, groupkey: metadata.GroupKey.ToString(), content: "Cannot find memory item!");
            }
        }

        #endregion

        #region Implement

        static void NewMemoryItem(dynamic metadata, dynamic content, MemoryType memory)
        {
            var text = content.Text.ToString();
            var parentId = content.ParentId.ToString();
            var id = metadata.ReferenceKey.ToString();
            var hint = content.Hint.ToString();
            if (!Memories.Any(t => t.Id == id || (t.ParentId == parentId && t.Text == text)))
            {

                AddMemoryItem(id, metadata.GroupKey.ToString(), text, hint, parentId, GetCreateDate(metadata), memory);
            }
            else
            {
                SendFeedbackMessage(type: FeedbackType.Error, actionTime: GetCreateDate(metadata), action: FeedbackActions.CannotAddMemory, groupkey: metadata.GroupKey.ToString(), content: "Cannot add dupicate memory item!");
            }
        }

        static void SendFeedbackMessage(FeedbackType type, string action, DateTimeOffset actionTime, string groupkey, dynamic content)
        {
            if (Program.StartingTimeApp < actionTime)
            {
                //Console.WriteLine($"{type}, {action}, {groupkey}, {content}");
                ProducerHelper.SendAMessage(
                        MessageTopic.MemoryFeedback,
                        new Feedback(type: type, name: FeedbackGroupNames.Memory, action: action, metadata: Helper.GetMetadataByGroupKey(groupkey), content: content)
                        )
                    .GetAwaiter().GetResult();
            }
        }

        static DateTimeOffset GetCreateDate(dynamic metadata)
        {
            return DateTimeOffset.Parse(metadata.CreateDate.ToString());
        }

        static void AddMemoryItem(string id, string groupKey, string text, string hint, string parentId, DateTimeOffset actionTime, MemoryType memoryType)
        {
            var memory = new MemoryItem { Id = id, ParentId = parentId, GroupKey = groupKey, Hint = hint, Text = text, MemoryType = memoryType };
            Memories.Add(memory);
            SendFeedbackMessage(type: FeedbackType.Success, actionTime: actionTime, action: FeedbackActions.NewMemoryAdded, groupkey: groupKey, content: memory);
        }

        #endregion

        internal static IEnumerable<MemoryItem> GetMemory(string groupKey)
        {
            return Memories.Where(m => m.GroupKey == groupKey);
        }

        #region Common actions

        public static void Reset(dynamic metadata, dynamic content)
        {
            Memories = new List<MemoryItem>();
        }

        public static List<MemoryItem> Memories { get; private set; } = new List<MemoryItem>();
        #endregion
    }
    public class MemoryItem
    {
        public string Id { get; internal set; }
        public string ParentId { get; internal set; }
        public string Text { get; internal set; }
        public string GroupKey { get; internal set; }
        public MemoryType MemoryType { get; internal set; }
        public string Hint { get; internal set; }
    }

    public enum MemoryType
    {
        Memory,
        Category
    }
}
