using System.Net.Http;
using System.Linq;
using System.Text;
//using Json.Net;
using System.Text.Json;
using NUnit.Framework;
using TechTalk.SpecFlow;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Threading;

namespace SpecFlowDemo.Steps
{
    [Binding]
    public sealed class TodoStepDefinitions
    {
        string selectedId = "";

        public TodoStepDefinitions()
        {
        }

        //[BeforeTestRun]
        //public static void BTR()
        //{
        //    RestHelper.MakeAGetRequest("https://localhost:5003/TodoQuery/Reset");
        //}

        [Given("I send the following task:")]
        public void WhenISendFllowingTasks(Table table)
        {
            RestHelper.MakeAGetRequest("https://localhost:5003/TodoQuery/Reset");
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            foreach (var row in table.Rows)
            {
                var content = new iTodo() { Description = row["TaskDesc"] };
                var msg = new Msg() { Action = "newTask", GroupKey = row["GroupKey"], Content = JsonSerializer.Serialize(content) };
                var dataToSend = JsonSerializer.Serialize(msg);
                RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
            }

        }


        [When("User select item (.*)")]
        public void WhenUserSelectItemN(string index)
        {
            //Thread.Sleep(1000);
            var url = "https://localhost:5003/TodoQuery?groupKey=All";
            var todos = RestHelper.MakeAGetRequest(url);
            //Thread.Sleep(1000);
            selectedId = todos.First().GetProperty("id").ToString();
        }

        [When("User update description to '(.*)' for '(.*)'")]
        public void WhenUserUpdateDescription(string newDescription, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId, Description = newDescription };
            var msg = new Msg() { Action = "updateDescription", GroupKey = groupKey, Content = JsonSerializer.Serialize(content) };
            var dataToSend = JsonSerializer.Serialize(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [When("User set deadline '(.*)' on selected task for '(.*)'")]
        public void WhenUserSetDeadline(string newDeadline, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId, Deadline = newDeadline };
            var msg = new Msg() { Action = "setDeadline", GroupKey = groupKey, Content = JsonSerializer.Serialize(content) };
            var dataToSend = JsonSerializer.Serialize(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [When("User set tag '(.*)' on selected task for '(.*)'")]
        public void WhenUserSetTag(string newTag, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId, Tag = newTag };
            var msg = new Msg() { Action = "setTag", GroupKey = groupKey, Content = JsonSerializer.Serialize(content) };
            var dataToSend = JsonSerializer.Serialize(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [Then("I should see the following todo list:")]
        public void ThenTheResultShouldBe(Table table)
        {
            dynamic[] todos = null;
            var tableColumns = table.Header.ToArray();
            var map = new Dictionary<string, string>
            {
                { "TaskDesc", "description" },
                { "GroupKey", "groupKey" },
                { "Deadline", "deadline" },
                { "Tags", "tags" },
            };
            var expectedColums = map.Where(k => tableColumns.Contains(k.Key)).Select(k => k.Value).ToArray();


            foreach (var row in table.Rows.GroupBy(r => r["GroupKey"]))
            {
                var url = $"https://localhost:5003/TodoQuery?groupKey={row.Key}";
                todos = RestHelper.MakeAGetRequest(url);
                AreEqual(RestHelper.DynamicToList(todos, expectedColums), row.ToList(tableColumns));
            }
        }

        void AreEqual(string[] expected, string[] values) => Assert.AreEqual(values.Joine(), expected.Joine());

    }

    internal class Msg
    {
        public string Action { get; set; }
        public string GroupKey { get; set; }
        public string Content { get; set; }
    }

    internal class iTodo
    {
        public iTodo()
        {
        }
        public string Description { get; set; }
    }

    class expectedResultItem
    {
        string Id { get; }
        public string Description { get; set; }
        public string GroupKey { get; set; }
        public string AssignedTo { get; set; }
        public List<string> Category { get; set; }
        public DateTime? Deadline { get; set; }
        public int Sequence { get; set; }

        public override string ToString()
        {
            return $"GroupKey: {GroupKey}";
        }
    }

    class expectedResult
    {
        public List<expectedResultItem> description { get; set; }
    }
}