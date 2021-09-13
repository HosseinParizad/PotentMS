using iMemory;
using NUnit.Framework;
using PotentHelper;
using SpecFlowDemo.Steps;
using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace iTest
{
    [Binding]
    class iMemoryTest
    {
        [Given(@"User Ask to Memorize '(.*)' for '(.*)'")]
        public void GivenUserAskToMemorizeFor(string text, string groupKey)
        {
            var parentId = "";
            var id = Guid.NewGuid().ToString();
            var hint = "";
            TestManager.Instance.MemoryDb.Add(TestHelper.BuildContent.Memory.NewMemory(groupKey, id, text, hint, parentId));
        }

        [Then(@"I send the following Memorize list:")]
        public void ThenISendTheFollowingMemorizeList(Table table)
        {
            var tableColumns = table.Header.ToArray();
            foreach (var row in table.Rows)
            {
                TestManager.Instance.MemoryDb.Add(TestHelper.BuildContent.Memory.NewMemory(GetValueFromRow(row, "GroupKey", tableColumns),
                    GetValueFromRow(row, "Id", tableColumns),
                    GetValueFromRow(row, "Text", tableColumns),
                    GetValueFromRow(row, "Hint", tableColumns),
                    GetValueFromRow(row, "ParentId", tableColumns)));
            }
        }

        string GetValueFromRow(TableRow row, string column, string[] tableColumns) => tableColumns.Contains(column) ? row[column] : "";

        [Given(@"User tell I Memorize '(.*)' for '(.*)'")]
        public void GivenUserTellIMemorizeFor(string id, string groupKey)
        {
            TestManager.Instance.MemoryDb.Add(TestHelper.BuildContent.Memory.Learnt(groupKey, id));
        }

        [Then(@"User delete memory '(.*)' for '(.*)'")]
        public void ThenUserDeleteMemoryFor(string id, string groupKey)
        {
            TestManager.Instance.MemoryDb.Add(TestHelper.BuildContent.Memory.Delete(groupKey, id));
        }

        [Then(@"I should see the following Memorize list:")]
        public void ThenIShouldSeeTheFollowingMemorizeList(Table table)
        {
            var tableColumns = table.Header.ToArray();

            foreach (var row in table.Rows.GroupBy(r => r["GroupKey"]))
            {
                var todos = new iMemory.Controllers.MemoryController(null).GetPresentation(row.Key.ToString()).ToArray();
                Assert.AreEqual(todos.Select(t => t.Text.Trim()).ToList(), row.Select(r => r["Text"]));
                if (tableColumns.Contains("Children"))
                {
                    Assert.AreEqual(todos.Select(t => t.Items.Count).ToList(), row.Select(r => int.Parse(r["Children"])));
                }
                //Assert.AreEqual(RestHelper.DynamicToList(todos, expectedColums), row.ToList(tableColumns, new Dictionary<string, string> { { "[selectedid]", SelectedId } }));
            }
        }


        //[Then(@"I get feedback '(.*)'")]
        //public void ThenIGetFeedbackTaskPaused(string expectedResult)
        //{
        //    Assert.AreEqual("TimeFeedback", LastMessage.Topic);
        //    Assert.AreEqual(expectedResult, LastMessage.Message.Action);
        //}
    }
}

