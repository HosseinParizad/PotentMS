using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace SpecFlowDemo.Steps
{
    [Binding]
    public class PersonalAssistantStepDefinitions
    {
        public PersonalAssistantStepDefinitions(ScenarioContext scenarioContext)
        {
        }

        [Then(@"I should see the following board for '(.*)':")]
        public void ThenIShouldSeeTheFollowingBoard(string groupKey, Table table)
        {
            System.Threading.Thread.Sleep(1000);
            dynamic[] dashboard = null;
            var tableColumns = table.Header.ToArray();
            var map = new Dictionary<string, string>
            {
                { "Text", "text" },
                { "Badges", "badges" },
            };

            var expectedColums = map.Where(k => tableColumns.Contains(k.Key)).Select(k => k.Value).ToArray();

            var url = $"https://localhost:5007/PersonalAssistant/{groupKey}";
            dashboard = RestHelper.MakeAGetRequest(url);
            Dictionary<String, string> replaceValues = new Dictionary<string, string>();
            if (tableColumns.Contains("Badges"))
            {
                foreach (var row in table.Rows)
                {
                    var badges = JsonSerializer.Deserialize<List<string>>(row["Badges"]);
                    int isLocation = row["Text"] == "UsedLocations" ? 1 : 0;
                    foreach (var item in badges)
                    {
                        replaceValues.Add($"\"{item}\"", DefaultBadgeItem(item, isLocation));
                    }

                }
            }
            RestHelper.AreEqual(RestHelper.DynamicToList(dashboard, expectedColums), table.ToList(tableColumns, replaceValues));
        }

        static string DefaultBadgeItem(string item, int isLocation)
            => "{" + $"\"text\":\"{item.Replace("\"", "")}\",\"link\":null,\"type\":{isLocation},\"count\":0" + "}";


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

        #region tag

        [Then(@"I should see the following tasks for selected tag '(.*)' for '(.*)':")]
        public void ThenIShouldSeeTheFollowingListForSelecedTag(string tag, string groupKey, Table table)
        {
            dynamic[] tasks = null;
            var tableColumns = table.Header.ToArray();
            var map = new Dictionary<string, string>
             {
                 { "Text", "description" },
             };

            var expectedColums = map.Where(k => tableColumns.Contains(k.Key)).Select(k => k.Value).ToArray();

            var url = $"https://localhost:5003/GetTaskByGroupTag/{groupKey}/{tag}";
            tasks = RestHelper.MakeAGetRequest(url);
            RestHelper.AreEqual(RestHelper.DynamicToList(tasks, expectedColums), table.ToList(tableColumns));
        }

        [Then(@"I should see the following tasks when '(.*)' go to '(.*)':")]
        public void ThenIShouldSeeTheFollowingListWhenMove(string groupKey, string tag, Table table)
        {
            dynamic[] tasks = null;
            var tableColumns = table.Header.ToArray();
            var map = new Dictionary<string, string>
             {
                 { "Text", "description" },
             };

            var expectedColums = map.Where(k => tableColumns.Contains(k.Key)).Select(k => k.Value).ToArray();

            var url = $"https://localhost:5003/GetTaskWhenMoveToLocation/{groupKey}/{tag}";
            tasks = RestHelper.MakeAGetRequest(url);
            RestHelper.AreEqual(RestHelper.DynamicToList(tasks, expectedColums), table.ToList(tableColumns));
        }

        #endregion
    }
}