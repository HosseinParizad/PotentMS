using System.Net.Http;
using System.Text;
using System.Text.Json;
using NUnit.Framework;
using TechTalk.SpecFlow;

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

        [When("i send a request to add task (.*) for (.*)")]
        public void WhenTheTwoNumbersAreAdded(string desc, string belongTo)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            using (var httpClient = new HttpClient(handler))
            {
                for (var i = 0; i < 1; i++)
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://localhost:5001/Gateway/"))
                    {
                        request.Headers.TryAddWithoutValidation("Accept", "application/json");

                        var content = new iTask() { Id = "1234", Description = desc };
                        var msg = new Msg() { Action = "newTask", BelongTo = belongTo, Content = JsonSerializer.Serialize(content) };
                        var mmm = JsonSerializer.Serialize(msg);
                        request.Content = new StringContent(mmm, Encoding.UTF8, "application/json");

                        var r = httpClient.Send(request);

                        //Assert.Null(r.StatusCode);
                    }
                }
            }
        }

        [Then("the result should be (.*)")]
        public void ThenTheResultShouldBe(int result)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://localhost:5003/ToDoQuery"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");

                    var response = httpClient.Send(request);

                    //Assert.Null(r.StatusCode);
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    Assert.NotNull(responseString);
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
        public string BelongTo { get; set; }
        public string Content { get; set; }
    }

    internal class iTask
    {
        public iTask()
        {
        }

        public string Id { get; set; }
        public string Description { get; set; }
    }
}