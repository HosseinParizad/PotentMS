using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using NUnit.Framework;
using PotentHelper;
using TechTalk.SpecFlow;

namespace SpecFlowDemo.Steps
{
    [Binding]
    public class GroupStepDefinitions
    {
        public GroupStepDefinitions(ScenarioContext scenarioContext)
        {

        }

        [BeforeStep]
        public virtual void BeforeStep()
        {
            //System.Threading.Thread.Sleep(1000);
        }

        [BeforeScenario]
        public virtual void BeforeScenario()
        {
            const string url = "https://localhost:5001/Gateway/Common";
            var httpMethod = HttpMethod.Post;

            var msg = new Msg(action: "reset", key: "Do not care", content: null);

            var dataToSend = JsonSerializer.Serialize(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [Given(@"Send an email '(.*)' to create group")]
        public void GivenSendAnEmailToCreateGroup(string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { GroupKey = groupKey };
            var msg = new Msg(action: "newGroup", key: groupKey, content: JsonSerializer.Serialize(content));
            var dataToSend = JsonSerializer.Serialize(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [Given(@"Add '(.*)' as member of '(.*)'")]
        public void GivenAddAsMemberOf(string member, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/";
            var httpMethod = HttpMethod.Post;

            var content = new { NewMember = member };
            var msg = new Msg(action: "newMember", key: groupKey, content: JsonSerializer.Serialize(content));

            var dataToSend = JsonSerializer.Serialize(msg);
            RestHelper.HttpMakeARequestWaitForFeedback(url, httpMethod, dataToSend);
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
                var expect = table.ToList(tableColumns);
                var result = RestHelper.DynamicToList(groups, expectedColums);
                //Assert.Multiple(() =>
                //{
                //    Assert.IsTrue(expect.All(result.Contains), String.Join(",", expect) + " -(o)- " + String.Join(",", result));
                //    Assert.IsTrue(result.All(expect.Contains), String.Join(",", expect) + " -(o)- " + String.Join(",", result));
                //});

                var set1 = new HashSet<string>(expect);
                var set2 = new HashSet<string>(result);
                Assert.Multiple(() =>
                {
                    Assert.IsFalse(expect.Except(result).Any(), String.Join(",", expect.Except(result)));
                    Assert.IsFalse(result.Except(expect).Any(), String.Join(",", expect.Except(expect)));
                });

            }
        }
    }
}