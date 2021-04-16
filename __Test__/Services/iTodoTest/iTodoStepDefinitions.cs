using System.Net.Http;
using System.Linq;
using System.Text;
//using Json.Net;
using System.Text.Json;
using NUnit.Framework;
using TechTalk.SpecFlow;
using System.Threading.Tasks;

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
                HttpMakeARequest(table, url, httpMethod, dataToSend);
            }

        }

        private static void HttpMakeARequest(Table table, string url, HttpMethod httpMethod, string dataToSend)
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

        [Then("I should see the following todo list:")]
        public void ThenTheResultShouldBe(TechTalk.SpecFlow.Table table)
        {
            dynamic todos = table;
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            foreach (var row in table.Rows)
            {
                var r = row["GroupKey"];
                using (var httpClient = new HttpClient(handler, false))
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:5003/TodoQuery?groupKey={r}"))
                    {
                        //request.Headers.TryAddWithoutValidation("Accept", "application/json");

                        //request.Content = new StringContent("", Encoding.UTF8, "application/json");
                        try
                        {
                            var response = httpClient.Send(request);
                            var responseString = response.Content.ReadAsStringAsync().Result;
                            //todos = JsonSerializer.Deserialize<object>(responseString);
                            todos = JsonSerializer.Deserialize<dynamic[]>(responseString);
                            //dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
                            //var pet = JsonSerializer.Deserialize<Pet>(responseString);

                        }
                        catch
                        {
                        }


                        //         //Assert.Null(r.StatusCode);
                        //         var responseString = response.Content.ReadAsStringAsync().Result;
                        //         todos = JsonSerializer.Deserialize<dynamic[]>(responseString);
                        //         //Assert.AreEqual(belongTo, d[0].GetProperty("assignedTo").ToString());
                        //         //Assert.AreEqual(desc, d[0].GetProperty("description").ToString());
                    }
                }
            }
            //foreach (var row in table.Rows)
            //{
            //    Assert.AreEqual(true, todos.Single(t=> t.Description == row["TaskDesc"]))
            //}
            //Assert.AreEqual(todos[0].description,"");
            //RestHelper.GetObjects<dynamic>("");
            //var url = new Uri(r)
            //var myObject = await RestClient.GetObjects<dynamic>(url);

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
}