using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Mcdonalds.AM.ApiCaller
{
    public class ApiProxy
    {
        public static CookieContainer ApiCookieContainer { get; set; }
        public static void SetCookies(string domain,HttpCookieCollection httpCookies)
        {
            ApiCookieContainer = new CookieContainer();
            foreach (string cookieKey in httpCookies)
            {
                Cookie cookie = new Cookie();
                HttpCookie httpCookie = httpCookies[cookieKey];
                cookie.Domain = domain;
                cookie.Expires = httpCookie.Expires;
                cookie.Name = httpCookie.Name;
                cookie.Path = httpCookie.Path;
                cookie.Secure = httpCookie.Secure;
                cookie.Value = httpCookie.Value;
                ApiCookieContainer.Add(cookie);
            }
        }
        public static string Call(string url, string method, NameValueCollection queryString, byte[] formData)
        {
            var _steam = RequestUrl(url, method, queryString, formData);
            string _result = Encoding.UTF8.GetString(_steam);
            return _result;
        }

        public static T Call<T>(string url, string method, NameValueCollection queryString, byte[] formData) where T : class
        {
            return JsonConvert.DeserializeObject<T>(Call(url, method, queryString, formData));
        }

        private static byte[] RequestUrl(string url, string method, NameValueCollection queryString, byte[] formData)
        {
            if (queryString != null)
            {
                foreach (string key in queryString)
                {
                    if (key.ToLower() != "url")
                    {
                        if (url.IndexOf("?") >= 0)
                        {
                            url += "&";
                        }
                        else
                        {
                            url += "?";
                        }
                        url += string.Format("{0}={1}", key, queryString[key]);
                    }
                }
            }
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.CookieContainer = ApiCookieContainer;
            request.Referer = "about:blank";
            request.ContentType = "application/json";
            request.Method = method;
            if (formData != null && formData.Length > 0)
            {
                Stream stream = request.GetRequestStream();
                stream.Write(formData, 0, formData.Length);
            }
            Stream smResponse = request.GetResponse().GetResponseStream();
            using (MemoryStream ms = new MemoryStream())
            {
                smResponse.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static T Call<T>(string uri) where T : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    HttpResponseMessage response = client.GetAsync(uri).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObjectAsync<T>(result).Result;
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
            catch
            {
                return null; 
            }
        }
    }
}