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

namespace SpecFlowDemo.Steps
{
    [Binding]
    public sealed class AdditionStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private int num1 { get; set; }
        private int num2 { get; set; }

        public AdditionStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BTR()
        {
            //            var i = @"/Users/hosseinparizad/kafka_2.13-2.7.0/bin/kafka-run-class.sh kafka.tools.GetOffsetShell \
            //--broker-list localhost:9092 \
            //--topic my-topic \
            //--time -1";

            //RunProcess("--alter --add-config retention.ms=100");

            //var startInfo = new ProcessStartInfo()
            //{
            //    FileName = @"/Users/hosseinparizad/kafka_2.13-2.7.0/bin/kafka-topics.sh",
            //    Arguments = string.Format("{0} {1} {2} {3} {4}", "--delete --zookeeper localhost:2181 --topic task", "", "", "", ""),
            //    UseShellExecute = false,
            //    RedirectStandardOutput = true,
            //    CreateNoWindow = true
            //};
            //var proc = new Process()
            //{
            //    StartInfo = startInfo,
            //};
            //proc.Start();
            //while (!proc.StandardOutput.EndOfStream)
            //{
            //    var r = proc.StandardOutput.ReadLine();
            //    //do something here
            //}
            //proc.Start();
            //while (!proc.StandardOutput.EndOfStream)
            //{
            //    var r = proc.StandardOutput.ReadLine();
            //    //do something here
            //}
            RestHelper.MakeAGetRequest("https://localhost:5003/TodoQuery/Reset");
        }

        //static void RunProcess(string secParam)
        //{
        //    var startInfo = new ProcessStartInfo()
        //    {
        //        FileName = @"/Users/hosseinparizad/kafka_2.13-2.7.0/bin/kafka-configs.sh",
        //        Arguments = string.Format("{0} {1} {2} {3} {4}", "--zookeeper localhost:2181 --entity-type topics --entity-name task", secParam, "", "", ""),
        //        UseShellExecute = false,
        //        RedirectStandardOutput = true,
        //        CreateNoWindow = true
        //    };
        //    var proc = new Process()
        //    {
        //        StartInfo = startInfo,
        //    };
        //    proc.Start();
        //    while (!proc.StandardOutput.EndOfStream)
        //    {
        //        proc.StandardOutput.ReadLine();
        //        //do something here
        //    }
        //}

        //[AfterTestRun]
        //public static void ATR()
        //{
        //    //            var i = @"/Users/hosseinparizad/kafka_2.13-2.7.0/bin/kafka-run-class.sh kafka.tools.GetOffsetShell \
        //    //--broker-list localhost:9092 \
        //    //--topic my-topic \
        //    //--time -1";
        //    //RunProcess("--time -1");
        //    //RunProcess("--alter --add-config retention.ms=100");
        //}

        [When("I send the following task:")]
        public void WhenTheTwoNumbersAreAdded(TechTalk.SpecFlow.Table table)
        {
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

        [Then("I should see the following todo list:")]
        public void ThenTheResultShouldBe(TechTalk.SpecFlow.Table table)
        {
            dynamic[] todos = null;
            //foreach (var row in table.Rows)
            //{
            //    var url = $"https://localhost:5003/TodoQuery?groupKey={row["GroupKey"]}";
            //    todos = MakeAGetRequest(url);
            //}

            var url = "https://localhost:5003/TodoQuery?groupKey=All";
            todos = RestHelper.MakeAGetRequest(url);
            AreEqual(RestHelper.DynamicToList(todos, new string[] { "description", "groupKey" }), table.ToList(new string[] { "TaskDesc", "GroupKey" }));

        }

        void AreEqual(string[] expected, string[] values) => Assert.AreEqual(expected.Joine(), values.Joine());

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