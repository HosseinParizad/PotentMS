using iGroup;
using NUnit.Framework;
using PotentHelper;
using SpecFlowDemo.Steps;
using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace iTest
{
    [Binding]
    class iGroupTest
    {
        [Then(@"I send the following Group list:")]
        public void ThenISendTheFollowingGroupList(Table table)
        {
            var tableColumns = table.Header.ToArray();
            foreach (var row in table.Rows)
            {
                TestManager.Instance.GroupDb.Add(TestHelper.BuildContent.Group.NewGroup(
                    GetValueFromRow(row, "Group", tableColumns),
                    GetValueFromRow(row, "Group", tableColumns)));
            }
        }
        string GetValueFromRow(TableRow row, string column, string[] tableColumns) => tableColumns.Contains(column) ? row[column] : "";

        [Then(@"I should see the following Group list:")]
        public void ThenIShouldSeeTheFollowingGroupList(Table table)
        {
            var tableColumns = table.Header.ToArray();

            foreach (var row in table.Rows.GroupBy(r => r["GroupKey"]))
            {
                var todos = new iGroup.Controllers.GroupController(null).GetPresentation(row.Key.ToString()).ToArray();
                Assert.AreEqual(todos.Select(t => t.Text.Trim()).ToList(), row.Select(r => r["Text"]));
            }
        }


        [Then(@"Use add member '(.*)' to group '(.*)'")]
        public void ThenUseAddMemberToGroup(string newMember, string groupKey)
        {
            TestManager.Instance.GroupDb.Add(TestHelper.BuildContent.Group.AddMember(groupKey, newMember));
        }

        [When(@"I update group '(.*)' to '(.*)'")]
        public void WhenIUpdateGroupTo(string groupKey, string newGroupName)
        {
            TestManager.Instance.GroupDb.Add(TestHelper.BuildContent.Group.UpdateGroup(groupKey, newGroupName));
        }

        [When(@"I delete group '(.*)'")]
        public void WhenIDeleteGroupTo(string groupName)
        {
            TestManager.Instance.GroupDb.Add(TestHelper.BuildContent.Group.DeleteGroup(groupName));
        }

    }
}

