using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using PotentHelper;

namespace SpecFlowDemo.Steps
{
    public static class RestHelper
    {
        //public static async Task<List<T>> GetObjects<T>(Uri uri)
        //{
        //    var client = new HttpClient();
        //    client.MaxResponseContentBufferSize = int.MaxValue;
        //    var returnobj = new List<T>();
        //    try
        //    {
        //        var response = await client.GetAsync(uri);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var content = await response.Content.ReadAsStringAsync();
        //            returnobj = JsonConvert.DeserializeObject<List<T>>(content);
        //        }
        //    }
        //    catch
        //    {
        //        return new List<T>();
        //    }
        //    return returnobj;
        //}

        public static dynamic[] MakeAGetRequest(string url)
        {
            return MakeAGetRequestAsync(url).Result;
        }

        public static async Task<dynamic[]> MakeAGetRequestAsync(string url)
        {
            dynamic[] todos = null;
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            using (var httpClient = new HttpClient(handler, false))
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                try
                {
                    var data = await httpClient.GetStringAsync(url);
                    //return Helper.Deserialize(data, typeof(dynamic[]));
                    return Helper.DeserializeObject<dynamic[]>(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return todos;
        }

        public static void HttpMakeARequestWaitForFeedback(string url, HttpMethod httpMethod, string dataToSend)
        {
            const string urlFeedBack = "https://localhost:5001/Gateway/Feedback";
            const string urlPAFeedBack = "https://localhost:5001/Gateway/PAFeedback";

            var curPaFeedbackCount = RestHelper.MakeAGetRequest(urlPAFeedBack)?.Count() ?? 0;
            var curTaskFeedbackCount = RestHelper.MakeAGetRequest(urlFeedBack)?.Count() ?? 0;
            //System.Threading.Thread.Sleep(100);
            HttpMakeARequest(url, httpMethod, dataToSend);

            System.Threading.Thread.Sleep(100); // Todo: need to be fixed

            var i = 0;
            while (((RestHelper.MakeAGetRequest(urlPAFeedBack)?.Count() ?? 0) == curPaFeedbackCount
                || (RestHelper.MakeAGetRequest(urlFeedBack)?.Count() ?? 0) == curTaskFeedbackCount)
                && i < 3)
            {
                System.Threading.Thread.Sleep(10);
                i++;
            }
        }


        public static void HttpMakeARequest(string url, HttpMethod httpMethod, string dataToSend)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            using (var httpClient = new HttpClient(handler))
            using (var request = new HttpRequestMessage(httpMethod, url))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");

                request.Content = new StringContent(dataToSend, Encoding.UTF8, "application/json");

                var r = httpClient.Send(request);

                Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
            }
        }


        public static string Joine(this string[] s) => string.Join(",", s).Replace("\r", "").Replace("\n", "").Replace("    ", "").Replace("   ", "").Replace("  ", "").Replace(" ", ""); //Todo: Remove
        public static string Joine(this IEnumerable<string> s) => string.Join(",", s).Replace("\r", "").Replace("\n", "").Replace("    ", "").Replace("   ", "").Replace("  ", "").Replace(" ", ""); //Todo: Remove
        public static string Joine(this IEnumerable<string> s, Dictionary<string, string> replaceValues)
        {
            var result = string.Join(",", s);
            foreach (var rv in replaceValues)
            {
                result = result.Replace(rv.Key, rv.Value);
            }
            return result;
        }
        public static string Joine(this IEnumerable<dynamic> s) => string.Join(",", s.Select(i => i.ToString()));
        public static void AreEqual(string[] expected, string[] values) => Assert.AreEqual(values.Joine(), expected.Joine());
        public static string[] ToList(this Table table, string[] columns) => table.Rows.Select(r => columns.Select(e => r[e]).Joine()).ToArray();
        public static string[] ToList(this Table table, string[] columns, Dictionary<string, string> replaceValues) => table.Rows.ToList(columns, replaceValues);
        public static string[] ToList(this IEnumerable<TableRow> rows, string[] columns) => rows.Select(r => columns.Select(e => r[e]).Joine()).ToArray();
        public static string[] ToList(this IEnumerable<TableRow> rows, string[] columns, Dictionary<string, string> replaceValues) => rows.Select(r => columns.Select(e => r[e]).Joine(replaceValues)).ToArray();
        public static string[] DynamicToList(dynamic[] dynamicArray, string[] columns) => dynamicArray.Select(l => columns.Select(n => l.GetValue(n)).Joine()).Cast<string>().ToArray();
    }
}