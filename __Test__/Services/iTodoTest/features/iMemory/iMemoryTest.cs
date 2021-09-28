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
        [Given(@"User Ask to Memorize '(.*)' for '(.*)' in group '(.*)'")]
        public void GivenUserAskToMemorizeFor(string text, string memberKey, string groupKey)
        {
            var parentId = "";
            var id = Guid.NewGuid().ToString();
            var hint = "";
            TestManager.Instance.MemoryDb.Add(TestHelper.BuildContent.Memory.NewMemory(groupKey, memberKey, id, text, hint, parentId));
        }

        [Then(@"I send the following Memorize list:")]
        public void ThenISendTheFollowingMemorizeList(Table table)
        {
            var tableColumns = table.Header.ToArray();
            foreach (var row in table.Rows)
            {
                TestManager.Instance.MemoryDb.Add(TestHelper.BuildContent.Memory.NewMemory(GetValueFromRow(row, "GroupKey", tableColumns), GetValueFromRow(row, "MemberKey", tableColumns),
                    GetValueFromRow(row, "Id", tableColumns),
                    GetValueFromRow(row, "Text", tableColumns),
                    GetValueFromRow(row, "Hint", tableColumns),
                    GetValueFromRow(row, "ParentId", tableColumns)));
            }
        }

        string GetValueFromRow(TableRow row, string column, string[] tableColumns) => tableColumns.Contains(column) ? row[column] : "";

        [Given(@"User tell I Memorize '(.*)' for '(.*)' in '(.*)'")]
        public void GivenUserTellIMemorizeFor(string id, string memberKey, string groupKey)
        {
            TestManager.Instance.MemoryDb.Add(TestHelper.BuildContent.Memory.Learnt(groupKey, memberKey, id));
        }

        [Then(@"User delete memory '(.*)' for '(.*)' in '(.*)'")]
        public void ThenUserDeleteMemoryFor(string id, string memberKey, string groupKey)
        {
            TestManager.Instance.MemoryDb.Add(TestHelper.BuildContent.Memory.Delete(groupKey, memberKey, id));
        }

        [Then(@"I should see the following Memorize list:")]
        public void ThenIShouldSeeTheFollowingMemorizeList(Table table)
        {
            AssertMemoryList(table);
        }

        [Then(@"I should see memorize list after (.*) days:")]
        public void ThenIShouldSeeAfterDays(int days, Table table)
        {
            Engine.Now = Engine.Now.AddDays(days);
            AssertMemoryList(table);
        }

        static void AssertMemoryList(Table table)
        {
            var tableColumns = table.Header.ToArray();
            foreach (var row in table.Rows.GroupBy(r => new { GroupKey = r["GroupKey"].ToString(), MemberKey = r["MemberKey"].ToString() }))
            {
                var todos = new iMemory.Controllers.MemoryController(null).GetPresentation(row.Key.GroupKey, row.Key.MemberKey).ToArray();
                Assert.AreEqual(todos.Select(t => t.Text.Trim()).ToList(), row.Select(r => r["Text"]));
                if (tableColumns.Contains("Children"))
                {
                    Assert.AreEqual(todos.Select(t => t.Items.Count).ToList(), row.Select(r => int.Parse(r["Children"])));
                }
            }
        }
    }
}

