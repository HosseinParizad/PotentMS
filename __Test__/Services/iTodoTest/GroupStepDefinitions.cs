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
    public class GroupStepDefinitions
    {
        public GroupStepDefinitions(ScenarioContext scenarioContext)
        {
        }
        [Given(@"Send an email '(.*)' to create group")]
        public void GivenSendAnEmailToCreateGroup(string groupKey)
        {
            RestHelper.MakeAGetRequest("https://localhost:5003/TodoQuery/Reset");
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { GroupKey = groupKey };
            var msg = new Msg() { Action = "newGroup", GroupKey = groupKey, Content = JsonSerializer.Serialize(content) };
            var dataToSend = JsonSerializer.Serialize(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [Given(@"Add '(.*)' as member of '(.*)'")]
        public void GivenAddAsMemberOf(string member, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { NewMember = member };
            var msg = new Msg() { Action = "newMember", GroupKey = groupKey, Content = JsonSerializer.Serialize(content) };
            var dataToSend = JsonSerializer.Serialize(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [Then(@"I should see the following groups:")]
        public void ThenIShouldSeeTheFollowingGroups(Table table)
        {
            var url = $"https://localhost:5003/TodoQuery/GroupQuery?groupKey=All";
            var groups = RestHelper.MakeAGetRequest(url);
            var tableColumns = table.Header.ToArray();
            var map = new Dictionary<string, string>
            {
                { "Group", "group" },
                { "Member", "member" },
            };
            var expectedColums = map.Where(k => tableColumns.Contains(k.Key)).Select(k => k.Value).ToArray();

            foreach (var row in table.Rows)
            {
                Assert.IsTrue(RestHelper.DynamicToList(groups, expectedColums).All(table.ToList(tableColumns).Contains));
            }
        }
    }
}