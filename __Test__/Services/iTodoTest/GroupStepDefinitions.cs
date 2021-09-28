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
            //Reset("https://localhost:5001/Gateway");
            //Reset("https://localhost:5001/Gateway/Group");
            //Reset("https://localhost:5001/Gateway/Memory");
        }

        static void Reset(string url)
        {
            var httpMethod = HttpMethod.Post;
            var msg = new Msg(action: "reset", metadata: new { Test = "Test" }, content: new { Test = "Test" });

            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [Given(@"Send an email '(.*)' to create group")]
        public void GivenSendAnEmailToCreateGroup(string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/Group";
            var httpMethod = HttpMethod.Post;

            var content = new { Group = groupKey };
            var msg = new Msg(action: "newGroup", metadata: Helper.GetMetadataByGroupKey(groupKey, ""), content: content);
            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequest(url, httpMethod, dataToSend);
        }

        [Given(@"Add '(.*)' as member of '(.*)'")]
        public void GivenAddAsMemberOf(string member, string groupKey)
        {
            const string url = "https://localhost:5001/Gateway/Group";
            var httpMethod = HttpMethod.Post;

            var content = new { NewMember = member };
            var msg = new Msg(action: MapAction.Group.NewMember, metadata: Helper.GetMetadataByGroupKey(groupKey, ""), content: content);

            var dataToSend = JsonConvert.SerializeObject(msg);
            RestHelper.HttpMakeARequestWaitForFeedback(url, httpMethod, dataToSend);
        }

        [Given(@"Wait (.*)")]
        public void GivenWait(int n)
        {
            System.Threading.Thread.Sleep(n);
        }

        [Then(@"I should see the following groups:")]
        public void ThenIShouldSeeTheFollowingGroups(Table table)
        {
            ThenIShouldSeeTheFollowingGroup(table.Rows[0]["Group"], table);
        }


        [Then(@"I should see the following group '(.*)'")]
        public void ThenIShouldSeeTheFollowingGroup(string group, Table table)
        {
            var url = $"https://localhost:5012/Group/GetGroupsTestOnly?groupKey=" + group;
            var groups = RestHelper.MakeAGetRequest(url);
            var tableColumns = table.Header.ToArray();
            var map = new Dictionary<string, string>
            {
                { "Group", "GroupKey" },
                { "Member", "MemberKey" },
            };
            var expectedColums = map.Where(k => tableColumns.Contains(k.Key)).Select(k => k.Value).ToArray();

            foreach (var row in table.Rows)
            {
                var expect = table.ToList(tableColumns);
                var result = RestHelper.DynamicToList(groups, expectedColums);

                var set1 = new HashSet<string>(expect);
                var set2 = new HashSet<string>(result);
                Assert.Multiple(() =>
                {
                    //Assert.IsFalse(expect.Except(result).Any(), String.Join(",", expect.Except(result)));
                    //Assert.IsFalse(result.Except(expect).Any(), String.Join(",", expect.Except(expect)));
                    Assert.IsTrue(set1.SetEquals(set2));
                });

            }
        }
    }
}