using iTodo.Controllers;
using NUnit.Framework;
using PotentHelper;
using SpecFlowDemo.Steps;
using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace iTest.Task
{
    [Binding]
    class iTodoTest
    {
        string GroupKey = Guid.NewGuid().ToString();
        string SelectedId = Guid.NewGuid().ToString();

        [Given(@"I send to do items messages:")]
        public void GivenISendToDoItemsMessages(Table table)
        {
            foreach (var row in table.Rows)
            {
                TestManager.Instance.TodoDb.Add(TestHelper.BuildContent.Task.NewTask(row["GroupKey"], row["MemberKey"], row["Id"], row["Description"], row["ParentId"]));
            }
        }

        [When(@"Update '(.*)' task description to '(.*)' for '(.*)'")]
        public void WhenUpdateSelectedDescriptionriptionToFor(string id, string newDesciption, string groupKey)
        {
            TestManager.Instance.TodoDb.Add(TestHelper.BuildContent.Task.UpdateTask(groupKey, id, newDesciption));
        }

        [When(@"Move task '(.*)' to '(.*)' for '(.*)' in group '(.*)'")]
        public void WhenMoveTaskTo(string id, string newParentId, string memberKey, string groupKey)
        {
            TestManager.Instance.TodoDb.Add(TestHelper.BuildContent.Task.MoveTask(groupKey, memberKey, id, newParentId));
        }

        [When(@"Delete task '(.*)' for '(.*)' in group '(.*)'")]
        public void WhenDeleteTask(string id, string memberKey, string groupKey)
        {
            TestManager.Instance.TodoDb.Add(TestHelper.BuildContent.Task.DeleteTask(groupKey, memberKey, id));
        }

        [Then(@"I should see the following todo list directly:")]
        public void ThenIShouldSeeTheFollowingTodoListDirectly(Table table)
        {
            var tableColumns = table.Header.ToArray();
            foreach (var row in table.Rows.GroupBy(r => new { GroupKey = r["GroupKey"].ToString(), MemberKey = r["MemberKey"].ToString() }))
            {
                var i = 0;
                var todos = new TodoQueryController(null).GetPresentationTask(row.Key.GroupKey, row.Key.MemberKey).ToArray();
                Assert.AreEqual(todos.Select(t => t.Text).ToList(), row.Select(r => r["Text"]));
                var column = "Children";
                if (tableColumns.Contains(column))
                {
                    Assert.AreEqual(todos.Select(t => t.Items.Count).ToList(), row.Select(r => int.Parse(r[column])));
                }

                column = "Info";
                if (tableColumns.Contains(column))
                {
                    //foreach (var item in row)
                    //{
                    //    Assert.AreEqual(todos[i].Info.ToString(), item[column].ToString());
                    //    i++;
                    //}
                    Assert.AreEqual(todos.Select(t => t.Info.ToString()).ToList(), row.Select(r => r[column].ToString()));
                }
                //Assert.AreEqual(RestHelper.DynamicToList(todos, expectedColums), row.ToList(tableColumns, new Dictionary<string, string> { { "[selectedid]", SelectedId } }));

            }
        }

    }
}


