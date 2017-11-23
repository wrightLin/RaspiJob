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
    public class LoggerHelper
    {
        public RequestHelper requestHelper;


        public LoggerHelper()
        {
            requestHelper = new RequestHelper();
        }  



        /// <summary>
        /// 將樹莓派開門時的資訊透過 WebApi寫 log
        /// </summary>
        /// <param name="info"></param>
        public void PostUWPLogByApi(LogInfo info)
        {
            string logInfoJsonString = JsonConvert.SerializeObject(info);
            string postLogUrl = App.Config.PostInfoMessage2LogUrl;

            requestHelper.DoPostRequest(postLogUrl, logInfoJsonString);

        }
    }
}
