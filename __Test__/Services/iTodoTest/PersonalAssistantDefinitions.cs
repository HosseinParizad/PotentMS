﻿using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;

namespace SpecFlowDemo.Steps
{
    [Binding]
    public class PersonalAssistantStepDefinitions
    {
        public PersonalAssistantStepDefinitions(ScenarioContext scenarioContext)
        {
        }

        [Then(@"I should see the following board:")]
        public void ThenIShouldSeeTheFollowingBoard(Table table)
        {
            dynamic[] dashboard = null;
            var tableColumns = table.Header.ToArray();
            var map = new Dictionary<string, string>
            {
                { "AssistantKey", "assistantKey" },
                { "Text", "text" },
                { "Badges", "badges" },
            };

            var expectedColums = map.Where(k => tableColumns.Contains(k.Key)).Select(k => k.Value).ToArray();

            foreach (var row in table.Rows.GroupBy(r => r["AssistantKey"]))
            {
                var url = $"https://localhost:5007/PersonalAssistant/{row.Key}";
                dashboard = RestHelper.MakeAGetRequest(url);
                RestHelper.AreEqual(RestHelper.DynamicToList(dashboard, expectedColums), row.ToList(tableColumns));
            }
        }


        [Then(@"I should see the following board deallines:")]
        public void ThenIShouldSeeTheFollowingBoardDeadlines(Table table)
        {
            dynamic[] deadlines = null;
            var tableColumns = table.Header.Where(h => h != "GroupKey").ToArray();
            var map = new Dictionary<string, string>
            {
                { "Text", "text" },
                { "Deadline", "deadline" },
            };

            var expectedColums = map.Where(k => tableColumns.Contains(k.Key)).Select(k => k.Value).ToArray();

            foreach (var row in table.Rows.GroupBy(r => r["GroupKey"]))
            {
                var url = $"https://localhost:5007/Deadlines/{row.Key}";
                deadlines = RestHelper.MakeAGetRequest(url);
                RestHelper.AreEqual(RestHelper.DynamicToList(deadlines, expectedColums), row.ToList(tableColumns));
            }

        }
    }
}