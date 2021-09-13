using Newtonsoft.Json;
using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using Time = iTime;
using iTodo.Controllers;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SpecFlowDemo.Steps;

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
            //var groupKey = table.Rows[0]["GroupKey"];
            //todoTodo.db.
            //dynamic[] todos = null;
            var tableColumns = table.Header.ToArray();
            //var map = new Dictionary<string, string>
            //{
            //    { "Description", "Text" },
            //    //{ "GroupKey", "GroupKey" },
            //    //{ "Deadline", "Deadline" },
            //    //{ "Tags", "Tags" },
            //    //{ "Locations", "Locations" },
            //    //{ "ParentId", "ParentId" },
            //};
            //var expectedColums = map.Where(k => tableColumns.Contains(k.Key)).Select(k => k.Value).ToArray();

            foreach (var row in table.Rows.GroupBy(r => r["GroupKey"]))
            {
                var todos = new TodoQueryController(null).GetPresentationTask(row.Key.ToString()).ToArray();
                Assert.AreEqual(todos.Select(t => t.Text).ToList(), row.Select(r => r["Text"]));
                if (tableColumns.Contains("Children"))
                {
                    Assert.AreEqual(todos.Select(t => t.Items.Count).ToList(), row.Select(r => int.Parse(r["Children"])));
                }
                //Assert.AreEqual(RestHelper.DynamicToList(todos, expectedColums), row.ToList(tableColumns, new Dictionary<string, string> { { "[selectedid]", SelectedId } }));
            }
        }

    }
}


