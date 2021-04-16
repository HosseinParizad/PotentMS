using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
 
namespace SpecFlowDemo.Steps
{
    public static class RestHelper
    {
        public static async Task<List<T>> GetObjects<T>(Uri uri)
        {
            var client = new HttpClient();
            client.MaxResponseContentBufferSize = int.MaxValue;
            var returnobj = new List<T>();
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    returnobj = JsonConvert.DeserializeObject<List<T>>(content);
                }
            }
            catch
            {
                return new List<T>();
            }
            return returnobj;
        }
    }
}