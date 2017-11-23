using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspJob.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace RaspJob.Helper
{
    public class RequestHelper
    {
        /// <summary>
        /// Do Http Get Request(BeaconWebApi)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string DoGetRequest(string url)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(App.Config.BeaconWebApiAuthHeadKey, App.Config.BeaconWebApiAuthHeadValue);
            HttpResponseMessage response = client.GetAsync(url).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Do Http Post Request(BeaconWebApi)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public void DoPostRequest(string url , string jsonString)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(App.Config.BeaconWebApiAuthHeadKey, App.Config.BeaconWebApiAuthHeadValue);
            StringContent content = new StringContent(jsonString, Encoding.UTF8,"application/json");

            client.PostAsync(url, content);

            return;
        }


    }
}
