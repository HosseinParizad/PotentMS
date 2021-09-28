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
            string url = "https://localhost:5001/Gateway/";
            Func<string, dynamic> content = (desc) => new iTodo() { Description = desc, ParentId = selectedId };
            WhenISendFllowingTodos(table, "newTask", url, content);
        }

        [Given("I send the following memory:")]
        public void WhenISendFllowingMemoriess(Table table)
        {
            string url = "https://localhost:5001/Gateway/Memory";
            Func<string, dynamic> content = (desc) => new { Text = desc, ParentId = selectedId, Hint = "" };
            WhenISendFllowingTodos(table, "newMemory", url, content);
        }

        void WhenISendFllowingTodos(Table table, string action, string url, Func<string, dynamic> getContent)
        {
            var httpMethod = HttpMethod.Post;

            foreach (var row in table.Rows)
            {
                var msg = new Msg(action: action, metadata: Helper.GetMetadataByGroupKey(row["GroupKey"], row["MemberKey"]), content: getContent(row["TaskDesc"]));
                var dataToSend = JsonConvert.SerializeObject(msg);
                RestHelper.HttpMakeARequestWaitForFeedback(url, httpMethod, dataToSend);
            }
        }

        [Given("Someone send the following task and group:")]
        public void WhenISendFllowingTaskAndGroups(Table table)
        {

            var httpMethod = HttpMethod.Post;

            foreach (var group in table.Rows.GroupBy(r => new { GroupKey = r["GroupKey"].ToString(), MemberKey = r["MemberKey"].ToString() }))
            {
                var msg = new Msg(action: MapAction.Group.NewGroup, metadata: Helper.GetMetadataByGroupKey(group.Key.GroupKey, group.Key.MemberKey), content: new { Group = group });
                var dataToSend = JsonConvert.SerializeObject(msg);
                RestHelper.HttpMakeARequestWaitForFeedback("https://localhost:5001/Gateway/Group", httpMethod, dataToSend);
            }

            string url = "https://localhost:5001/Gateway/";
            Func<string, dynamic> content = (desc) => new iTodo() { Description = desc, ParentId = selectedId };
            WhenISendFllowingTodos(table, "newTask", url, content);
        }

        [Given("I send the following goals:")]
        public void WhenISendFllowingGoals(Table table)
        {
            const string url = "https://localhost:5001/Gateway/Goal";
            var httpMethod = HttpMethod.Post;

            foreach (var row in table.Rows)
            {
                var content = new { Text = row["Goal"], ParentId = selectedId };
                var msg = new Msg(action: MapAction.Goal.NewGoal, metadata: Helper.GetMetadataByGroupKey(row["GroupKey"], row["MemberKey"]), content: content);
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
                selectedId = todos.Skip(i - 1).Take(1).Single().Id.ToString();
            }
        }

        [When("User select Copy")]
        public void UserCopyId()
        {
            CopyId = selectedId;
            Console.WriteLine(CopyId);
        }
        string CopyId;

        [When("User select Paste to group '(.*)' for member '(.*)'")]
        public void PasteTo(string groupKey, string memberKey)
        {
            Console.WriteLine(selectedId);
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = CopyId, ToParentId = selectedId };
            var msg = new Msg(action: "moveTask", metadata: Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);
            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [When("User update description to '(.*)' for '(.*)' in group '(.*)'")]
        public void WhenUserUpdateDescription(string newDescription, string memberKey, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId, Description = newDescription };
            var msg = new Msg(action: "updateDescription", metadata: Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);
            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [Then("I should see the following memory list:")]
        public void ThenTheMemoryResultShouldBe(Table table)
        {
            dynamic[] todos = null;
            var tableColumns = table.Header.ToArray();
            var map = new Dictionary<string, string>
            {
                { "TaskDesc", "Text" },
                { "GroupKey", "GroupKey" },
                { "ParentId", "ParentId" },
            };
            var expectedColums = map.Where(k => tableColumns.Contains(k.Key)).Select(k => k.Value).ToArray();

            foreach (var row in table.Rows.GroupBy(r => new { GroupKey = r["GroupKey"].ToString(), MemberKey = r["MemberKey"].ToString() }))
            {
                var url = $"https://localhost:5008/Memory?groupKey={row.Key}";
                todos = RestHelper.MakeAGetRequest(url);
                AreEqual(row.ToList(tableColumns, new Dictionary<string, string> { { "[selectedid]", selectedId } }), RestHelper.DynamicToList(todos, expectedColums));
            }
        }

        [Then("I should see the following goal list:")]
        public void ThenTheGoalResultShouldBe(Table table)
        {
            dynamic[] todos = null;
            var tableColumns = table.Header.ToArray();
            var map = new Dictionary<string, string>
            {
                { "TaskDesc", "Text" },
                { "GroupKey", "GroupKey" },
                { "ParentId", "ParentId" },
            };
            var expectedColums = map.Where(k => tableColumns.Contains(k.Key)).Select(k => k.Value).ToArray();

            foreach (var row in table.Rows.GroupBy(r => new { GroupKey = r["GroupKey"].ToString(), MemberKey = r["MemberKey"].ToString() }))
            {
                var url = $"https://localhost:5010/Goal?groupKey={row.Key}";
                todos = RestHelper.MakeAGetRequest(url);
                AreEqual(row.ToList(tableColumns, new Dictionary<string, string> { { "[selectedid]", selectedId } }), RestHelper.DynamicToList(todos, expectedColums));
            }
        }

        [When("User delete memory item (.*) for member '(.*)' in group '(.*)'")]
        public void WhenUserUpdateDescription(int index, string memberKey, string groupKey)
        {
            var urlRead = $"https://localhost:5008/Memory?groupKey={groupKey}";
            var todos = RestHelper.MakeAGetRequest(urlRead);
            if (todos.Count() >= index)
            {
                selectedId = todos.Skip(index - 1).Take(1).Single().Id.ToString();
            }
            const string url = "https://localhost:5001/Gateway/Memory";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId };
            var msg = new Msg(action: "delMemory", metadata: Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);
            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [When("User set deadline '(.*)' on selected task for '(.*)' in group '(.*)'")]
        public void WhenUserSetDeadline(string newDeadline, string memberKey, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId, Deadline = newDeadline };
            var msg = new Msg(action: "setDeadline", metadata: Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);

            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequestWaitForFeedback(url, httpMethod, dataToSend);
        }

        [When(@"User close selected task for '(.*)' in group '(.*)'")]
        public void WhenUseCloseSelectedTask(string memberKey, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId };
            var msg = new Msg(action: "closeTask", metadata: Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);

            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [When(@"User assign selected task to for '(.*)' in group '(.*)'")]
        public void WhenUseCloseSelectedTask(string assignTo, string memberKey, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId, AssignTo = assignTo };
            var msg = new Msg(action: "assignTask", metadata: Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);

            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [When("User add '(.*)' to tag (.*) on selected task for '(.*)' in group '(.*)'")]
        public void WhenUserSetTag(string newTag, string tagKey, string memberKey, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId, TagKey = tagKey, Tag = newTag };

            var msg = new Msg(action: "setTag", metadata: Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);

            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [When("User set location '(.*)' on selected task for '(.*)' in group '(.*)'")]
        public void WhenUserSetLocation(string location, string memberKey, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { Id = selectedId, Location = location };
            var msg = new Msg(action: "setLocation", metadata: Helper.GetMetadataByGroupKey(groupKey, memberKey), content: content);
            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [When("User '(.*)' go to '(.*)'")]
        public void WhenUserGetLocationEvent(string member, string currentLocation)
        {
            const string url = "https://localhost:5001/Gateway/Location";
            var httpMethod = HttpMethod.Post;

            var content = new { Member = member, Location = currentLocation };
            var msg = new Msg(action: "setCurrentLocation", metadata: Helper.GetMetadataByGroupKey(member, member), content: content);

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
                { "TaskDesc", "Description" },
                { "GroupKey", "GroupKey" },
                { "Deadline", "Deadline" },
                { "Tags", "Tags" },
                { "Locations", "Locations" },
                { "ParentId", "ParentId" },
            };
            var expectedColums = map.Where(k => tableColumns.Contains(k.Key)).Select(k => k.Value).ToArray();

            foreach (var row in table.Rows.GroupBy(r => new { GroupKey = r["GroupKey"].ToString(), MemberKey = r["MemberKey"].ToString() }))
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
        public string Hint { get; set; }
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