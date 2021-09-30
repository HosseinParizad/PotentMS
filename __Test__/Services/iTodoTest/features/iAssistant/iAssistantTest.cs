using iAssistant.Controllers;
using NUnit.Framework;
using SpecFlowDemo.Steps;
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
            foreach (var row in table.Rows.GroupBy(r => new { GroupKey = r["GroupKey"].ToString(), MemberKey = r["MemberKey"].ToString() }))
            {
                var todos = new AssistantController(null).GetPresentation(row.Key.GroupKey, row.Key.MemberKey).ToArray();
                Assert.AreEqual(todos.Select(t => t.Text).ToList(), row.Select(r => r["Text"]));
            }
        }

        [Then(@"Asstant should see following parts:")]
        public void ThenAsstantShouldSeeFollowingParts(Table table)
        {
            var tableColumns = table.Header.ToArray();
            var todos = new AssistantController(null).GetPresentation("what ever", "what ever").ToArray();
            Assert.AreEqual(todos.Select(t => t.Text).ToList(), table.Rows.Select(r => r["Text"]));
        }
    }
}


