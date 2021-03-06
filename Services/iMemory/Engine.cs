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
                SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: MapAction.MemoryFeedback.MemoryDeleted.Name, groupkey: metadata.GroupKey.ToString(), content: new { Id = id });
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: MapAction.MemoryFeedback.CannotFindMemory.Name, groupkey: metadata.GroupKey.ToString(), content: "Cannot find memory item!");
            }
        }

        #endregion

        #region LearnMemory

        internal static void LearnMemory(dynamic metadata, dynamic content)
        {
            var groupKey = metadata.GroupKey.ToString();
            var id = content.Id.ToString();
            var memoryItem = Memories.SingleOrDefault(d => d.Id == id);
            if (memoryItem != null)
            {
                var nextdate = DateTimeOffset.Now.AddDays(1);
                var stage = memoryItem.Stage;
                switch (memoryItem.Stage)
                {
                    case MemoryStage.Stage1:
                        nextdate = DateTimeOffset.Now.AddDays(1);
                        stage = MemoryStage.Stage2;
                        break;
                    case MemoryStage.Stage2:
                        nextdate = DateTimeOffset.Now.AddDays(3);
                        stage = MemoryStage.Stage3;
                        break;
                    case MemoryStage.Stage3:
                        nextdate = DateTimeOffset.Now.AddDays(7);
                        stage = MemoryStage.Stage4;
                        break;
                    case MemoryStage.Stage4:
                        nextdate = DateTimeOffset.Now.AddDays(14);
                        stage = MemoryStage.Stage5;
                        break;
                    case MemoryStage.Stage5:
                        nextdate = DateTimeOffset.Now.AddDays(30);
                        stage = MemoryStage.Stage6;
                        break;
                    case MemoryStage.Stage6:
                        nextdate = DateTimeOffset.MaxValue;
                        break;
                    default:
                        break;
                }
                memoryItem.NextMemorizeDate = nextdate;
                memoryItem.Stage = stage;
            }
        }

        #endregion

        internal static IEnumerable<PresentItem> GetMemoryPresentation(string groupKey, string parentid)
        {
            return Memories.Where(i => i.GroupKey == groupKey && i.ParentId == parentid).Select(i => MemoryToPresentation(groupKey, i));
        }

        static PresentItem MemoryToPresentation(string groupKey, MemoryItem mi)
        {
            var presentItem = new PresentItem
            {
                Id = mi.Id,
                Text = $" {mi.Text } ({mi.Stage}) ",
                Link = mi.Hint,
                Actions = GetActions(mi).ToList(),
                Items = GetMemoryPresentation(groupKey, mi.Id).ToList()
            };
            return presentItem;
        }

        static IEnumerable<PresentItemActions> GetActions(MemoryItem mi)
        {
            Func<string, string, dynamic, PresentItemActions> createStep = (text, action, content) =>
               new PresentItemActions
               {
                   Text = text,
                   Group = "/Memory",
                   Action = action,
                   Metadata = new { GroupKey = mi.GroupKey, ReferenceKey = Guid.NewGuid().ToString() },
                   Content = content
               };
            yield return createStep("step", MapAction.Memory.NewMemory.Name, new { Text = "[text]", Hint = "[hint]", ParentId = mi.Id });
            yield return createStep("update", MapAction.Memory.UpdateMemory.Name, new { Text = "[text]", Hint = "[hint]", Id = mi.Id });
            yield return createStep("delete", MapAction.Memory.DelMemory.Name, new { Id = mi.Id });
            yield return createStep("learnt", MapAction.Memory.LearntMemory.Name, new { Id = mi.Id });
        }

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
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: MapAction.MemoryFeedback.CannotAddMemory.Name, groupkey: metadata.GroupKey.ToString(), content: "Cannot add dupicate memory item!");
            }
        }

        static void SendFeedbackMessage(MsgType type, string action, DateTimeOffset actionTime, string groupkey, dynamic content)
        {
            if (Program.StartingTimeApp < actionTime)
            {
                ProducerHelper.SendAMessage(
                        MessageTopic.MemoryFeedback,
                        new Feedback(type: type, action: action, metadata: Helper.GetMetadataByGroupKey(groupkey), content: content)
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
            var memory = new MemoryItem { Id = id, ParentId = parentId, GroupKey = groupKey, Hint = hint, Text = text, MemoryType = memoryType, Stage = MemoryStage.Stage1 };
            Memories.Add(memory);
            SendFeedbackMessage(type: MsgType.Success, actionTime: actionTime, action: MapAction.MemoryFeedback.NewMemoryAdded.Name, groupkey: groupKey, content: memory);
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
        public MemoryStage Stage { get; internal set; }
        public DateTimeOffset NextMemorizeDate { get; internal set; }
    }

    public enum MemoryType
    {
        Memory,
        Category
    }

    public enum MemoryStage
    {
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Stage5,
        Stage6
    }

}
