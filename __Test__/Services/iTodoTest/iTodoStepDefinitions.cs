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

        [When("i add (.*) and (.*)")]
        public void WhenTheTwoNumbersAreAdded(int number1, int number2)
        {
            num1 = number1;
            num2 = number2;
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            using (var httpClient = new HttpClient(handler))
            {
                for (var i = 0; i < 1; i++)
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://localhost:5001/Gateway/"))
                    {
                        request.Headers.TryAddWithoutValidation("Accept", "application/json");

                        var content = new iTask() { Id = "1234", Description = "ggggg" };
                        var msg = new Msg() { Action = "newTask", BelongTo = "ali", Content = JsonSerializer.Serialize(content) };
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
            Assert.AreEqual((num1 + num2), result);
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