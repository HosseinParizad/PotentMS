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
                TestManager.Instance.TodoDb.Add(TestHelper.BuildContent.Task.NewTask(row["GroupKey"], row["Id"], row["Description"], row["ParentId"]));
            }
        }

        [When(@"Update '(.*)' task description to '(.*)' for '(.*)'")]
        public void WhenUpdateSelectedDescriptionriptionToFor(string id, string newDesciption, string groupKey)
        {
            TestManager.Instance.TodoDb.Add(TestHelper.BuildContent.Task.UpdateTask(groupKey, id, newDesciption));
        }

        [When(@"Move task '(.*)' to '(.*)' for '(.*)'")]
        public void WhenMoveTaskTo(string id, string newParentId, string groupKey)
        {
            TestManager.Instance.TodoDb.Add(TestHelper.BuildContent.Task.MoveTask(groupKey, id, newParentId));
        }

        [When(@"Delete task '(.*)' for '(.*)'")]
        public void WhenDeleteTask(string id, string groupKey)
        {
            TestManager.Instance.TodoDb.Add(TestHelper.BuildContent.Task.DeleteTask(groupKey, id));
        }

        [Then(@"I should see the following todo list directly:")]
        public void ThenIShouldSeeTheFollowingTodoListDirectly(Table table)
        {
            var tableColumns = table.Header.ToArray();

            foreach (var row in table.Rows.GroupBy(r => r["GroupKey"]))
            {
                var i = 0;
                var todos = new TodoQueryController(null).GetPresentationTask(row.Key.ToString()).ToArray();
                Assert.AreEqual(todos.Select(t => t.Text).ToList(), row.Select(r => r["Text"]));
                if (tableColumns.Contains("Children"))
                {
                    Assert.AreEqual(todos.Select(t => t.Items.Count).ToList(), row.Select(r => int.Parse(r["Children"])));
                }
                if (tableColumns.Contains("Info"))
                {
                    foreach (var item in row)
                    {
                        Assert.AreEqual(todos[i].Info.ToString(), item["Info"].ToString());
                        i++;
                    }
                }
                //Assert.AreEqual(RestHelper.DynamicToList(todos, expectedColums), row.ToList(tableColumns, new Dictionary<string, string> { { "[selectedid]", SelectedId } }));

            }
        }

    }
}


