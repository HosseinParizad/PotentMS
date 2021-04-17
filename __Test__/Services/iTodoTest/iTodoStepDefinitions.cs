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
                HttpMakeARequest(url, httpMethod, dataToSend);
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
            todos = MakeAGetRequest(url);
            Assert.AreEqual(todos.Select(l => l.GetProperty("description").ToString()).ToArray(), table.Rows.Select(r => r["TaskDesc"]).ToArray());
            //Assert.AreEqual(todos, "Do what you want");
        }

        static dynamic[] MakeAGetRequest(string url)
        {
            dynamic[] todos = null;
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            using (var httpClient = new HttpClient(handler, false))
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    try
                    {
                        var response = httpClient.Send(request);
                        var responseString = response.Content.ReadAsStringAsync().Result;
                        todos = JsonSerializer.Deserialize<dynamic[]>(responseString);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return todos;
        }

        static void HttpMakeARequest(string url, HttpMethod httpMethod, string dataToSend)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(httpMethod, url))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");

                    request.Content = new StringContent(dataToSend, Encoding.UTF8, "application/json");

                    var r = httpClient.Send(request);

                    //Assert.Null(r.StatusCode);
                }
            }
        }

    }

    internal class Msg
    {
        public Msg()
        {
        }

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