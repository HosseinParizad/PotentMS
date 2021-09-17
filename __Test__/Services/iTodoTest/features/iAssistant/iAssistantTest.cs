using iAssistant.Controllers;
using iGroup;
using NUnit.Framework;
using PotentHelper;
using SpecFlowDemo.Steps;
using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace iAssistant
{
    [Binding]
    class iAssistantTest
    {
        [Then(@"Asstant should ask me to do following tasks:")]
        public void ThenAsstantShouldAskMeToDoFollowingTasks(Table table)
        {
            var tableColumns = table.Header.ToArray();

            foreach (var row in table.Rows.GroupBy(r => r["GroupKey"]))
            {
                //var i = 0;
                var todos = new AssistantController(null).GetPresentation(row.Key.ToString()).ToArray();
                Assert.AreEqual(todos.Select(t => t.Text).ToList(), row.Select(r => r["Text"]));
                //if (tableColumns.Contains("Children"))
                //{
                //    Assert.AreEqual(todos.Select(t => t.Items.Count).ToList(), row.Select(r => int.Parse(r["Children"])));
                //}
                //if (tableColumns.Contains("Info"))
                //{
                //    foreach (var item in row)
                //    {
                //        Assert.AreEqual(todos[i].Info.ToString(), item["Info"].ToString());
                //        i++;
                //    }
                //}
            }
        }
    }
}


