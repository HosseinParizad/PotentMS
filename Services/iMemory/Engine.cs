using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;

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

        #region Implement

        static void NewMemoryItem(dynamic metadata, dynamic content, MemoryType memory)
        {
            var description = content.Description.ToString();
            var parentId = "";//content.ParentId.ToString();
            var id = metadata.ReferenceKey.ToString();
            if (!Memories.Any(t => t.Id == id || (t.ParentId == parentId && t.Description == description)))
            {
                AddMemoryItem(id, metadata.GroupKey.ToString(), description, parentId, memory);
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
                        MessageTopic.TaskFeedback,
                        new Feedback(type: type, name: FeedbackGroupNames.Memory, action: action, metadata: Helper.GetMetadataByGroupKey(groupkey), content: content)
                        )
                    .GetAwaiter().GetResult();
            }
        }

        static DateTimeOffset GetCreateDate(dynamic metadata)
        {
            return DateTimeOffset.Parse(metadata.CreateDate.ToString());
        }

        static void AddMemoryItem(string id, string groupKey, string description, string parentId, MemoryType memoryType)
        {
            var memory = new MemoryItem { Id = id, ParentId = parentId, GroupKey = groupKey, Description = description, MemoryType = memoryType };
            Memories.Add(memory);
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

        public static List<MemoryItem> Memories { get; private set; }
        #endregion
    }
    public class MemoryItem
    {
        public string Id { get; internal set; }
        public string ParentId { get; internal set; }
        public string Description { get; internal set; }
        public string GroupKey { get; internal set; }
        public MemoryType MemoryType { get; internal set; }
    }

    public enum MemoryType
    {
        Memory,
        Category
    }
}
