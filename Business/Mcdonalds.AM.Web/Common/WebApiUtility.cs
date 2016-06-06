using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace Beyondbit.XHCultureOA.Utility.WebUtility
{
   public static class WebApiUtility
    {
        public static T GetWebApiData<T>(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(url).Result;
            var list = response.Content.ReadAsAsync<T>().Result;
            return list;
        }

       public static void GetWebApiData(string url)
       {
           HttpClient client = new HttpClient();
           
           HttpResponseMessage response = client.GetAsync(url).Result;
           var list = response.Content.ReadAsStringAsync();
       }

       public static T PostWebApiData<T, M>(string url, M entity)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<M>(
                    entity,
                    new JsonMediaTypeFormatter())
            };
           


            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(entity);
            //HttpContent content = new StringContent(json);
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.ContentLength = content.Headers.ContentLength;
            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri(url);
            //HttpResponseMessage response = client.PostAsync(url, content).Result;
            //T list = default(T);
            //if (response.IsSuccessStatusCode)
            //{
            //    list = response.Content.ReadAsAsync<T>().Result;
            //}

            T list = SubmitRequest<T>(request);
            return list;
        }

        /// <summary>
        /// Helper method to submit a request and print the request and response to the Console
        /// </summary>
        private static T SubmitRequest<T>(HttpRequestMessage request)
        {

            T list = default(T);
            // Create an HttpClient
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Issue a request
                    HttpResponseMessage response = client.SendAsync(request).Result;
           
                    if (response.Content != null)
                    {
                        list = response.Content.ReadAsAsync<T>().Result;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Request failed: {0}", e);
                }
            }
            return list;
        }
    }
}
