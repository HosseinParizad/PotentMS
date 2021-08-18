using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using NUnit.Framework;
using PotentHelper;
using TechTalk.SpecFlow;

namespace SpecFlowDemo.Steps
{
    [Binding]
    public sealed class TodoStepDefinitions
    {
        string selectedId = "";

        [Given("I send the following task:")]
        public void WhenISendFllowingTasks(Table table)
        {
            WhenISendFllowingTodos(table, "newTask");
        }

        [Given("I send the following memory:")]
        public void WhenISendFllowingMemoriess(Table table)
        {
            WhenISendFllowingTodos(table, "newMemory");
        }

        void WhenISendFllowingTodos(Table table, string action)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            foreach (var row in table.Rows)
            {
                var content = new iTodo() { Description = row["TaskDesc"], ParentId = selectedId };
                var msg = new Msg(action: action, metadata: Helper.GetMetadataByGroupKey(row["GroupKey"]), content: content);
                var dataToSend = JsonConvert.SerializeObject(msg);
                RestHelper.HttpMakeARequestWaitForFeedback(url, httpMethod, dataToSend);
            }
        }

        [Given("I send the following goals:")]
        public void WhenISendFllowingGoals(Table table)
        {
            const string url = "https://localhost:5001/Gateway";
            var httpMethod = HttpMethod.Post;

            foreach (var row in table.Rows)
            {
                var content = new iTodo() { Description = row["Goal"], ParentId = selectedId };
                var msg = new Msg(action: "newGoal", metadata: Helper.GetMetadataByGroupKey(row["GroupKey"]), content: content);
                var dataToSend = JsonConvert.SerializeObject(msg);
                RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
            }
        }

        [When("User select item (.*) from tasks of '(.*)'")]
        public void WhenUserSelectItemN(string index, string groupKey)
        {
            var url = $"https://localhost:5003/TodoQuery?groupKey={groupKey}";
            var i = int.Parse(index);
            var todos = RestHelper.MakeAGetRequest(url);
            if (todos.Count() >= i)
            {
                selectedId = todos.Skip(i - 1).Take(1).Single().id.ToString();
            }
        }

        [When("User update description to '(.*)' for '(.*)'")]
        public void WhenUserUpdateDescription(string newDescription, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId, Description = newDescription };
            var msg = new Msg(action: "updateDescription", metadata: Helper.GetMetadataByGroupKey(groupKey), content: content);
            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [When("User set deadline '(.*)' on selected task for '(.*)'")]
        public void WhenUserSetDeadline(string newDeadline, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId, Deadline = newDeadline };
            var msg = new Msg(action: "setDeadline", metadata: Helper.GetMetadataByGroupKey(groupKey), content: content);

            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequestWaitForFeedback(url, httpMethod, dataToSend);
        }

        [When(@"User close selected task for '(.*)'")]
        public void WhenUseCloseSelectedTask(string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId };
            var msg = new Msg(action: "closeTask", metadata: Helper.GetMetadataByGroupKey(groupKey), content: content);

            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [When(@"User assign selected task to '(.*)' for '(.*)'")]
        public void WhenUseCloseSelectedTask(string assignTo, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId, AssignTo = assignTo };
            var msg = new Msg(action: "assignTask", metadata: Helper.GetMetadataByGroupKey(groupKey), content: content);

            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [When("User add '(.*)' to tag (.*) on selected task for '(.*)'")]
        public void WhenUserSetTag(string newTag, string tagKey, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId, TagKey = tagKey, Tag = newTag };

            var msg = new Msg(action: "setTag", metadata: Helper.GetMetadataByGroupKey(groupKey), content: content);

            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [When("User set location '(.*)' on selected task for '(.*)'")]
        public void WhenUserSetLocation(string location, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId, Location = location };
            var msg = new Msg(action: "setLocation", metadata: Helper.GetMetadataByGroupKey(groupKey), content: content);
            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [When("User '(.*)' go to '(.*)'")]
        public void WhenUserGetLocationEvent(string member, string currentLocation)
        {
            const string url = "https://localhost:5001/Gateway/Location";
            var httpMethod = HttpMethod.Post;

            var content = new { Member = member, Location = currentLocation };
            var msg = new Msg(action: "setCurrentLocation", metadata: Helper.GetMetadataByGroupKey(member), content: content);

            var dataToSend = JsonConvert.SerializeObject(msg);
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
                { "Locations", "locations" },
                { "ParentId", "parentId" },
            };
            var expectedColums = map.Where(k => tableColumns.Contains(k.Key)).Select(k => k.Value).ToArray();

            foreach (var row in table.Rows.GroupBy(r => r["GroupKey"]))
            {
                var url = $"https://localhost:5003/TodoQuery?groupKey={row.Key}";
                todos = RestHelper.MakeAGetRequest(url);
                AreEqual(RestHelper.DynamicToList(todos, expectedColums), row.ToList(tableColumns, new Dictionary<string, string> { { "[selectedid]", selectedId } }));
            }
        }

        [Then(@"I should see feedback error '(.*)'")]
        public void ThenIShouldSeeValidationError(string errorMsg)
        {
            const string url = "https://localhost:5001/Gateway/Feedback";
            var hasExpectedError = RestHelper.MakeAGetRequest(url)?.Any(m => m.ToString().IndexOf(errorMsg) > -1) ?? false;

            Assert.True(hasExpectedError);
        }

        void AreEqual(string[] expected, string[] values) => Assert.AreEqual(values.Joine(), expected.Joine());

    }

    internal class iTodo
    {
        public string Description { get; set; }
        public string ParentId { get; set; }
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