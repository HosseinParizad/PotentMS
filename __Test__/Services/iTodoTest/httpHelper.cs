using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using TechTalk.SpecFlow;

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
            dynamic[] todos = null;
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            using (var httpClient = new HttpClient(handler, false))
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

            return todos;
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

                //Assert.Null(r.StatusCode);
            }
        }


        public static string Joine(this string[] s) => string.Join(",", s);
        public static string Joine(this IEnumerable<string> s) => string.Join(",", s);
        public static string Joine(this IEnumerable<dynamic> s) => string.Join(",", s.Select(i => i.ToString()));

        public static string[] ToList(this Table table, string[] columns) => table.Rows.Select(r => columns.Select(e => r[e]).Joine()).ToArray();
        public static string[] ToList(this IEnumerable<TableRow> rows, string[] columns) => rows.Select(r => columns.Select(e => r[e]).Joine()).ToArray();
        public static string[] DynamicToList(dynamic[] dynamicArray, string[] columns) => dynamicArray.Select(l => columns.Select(n => l.GetProperty(n)).Joine()).Cast<string>().ToArray();
    }
}