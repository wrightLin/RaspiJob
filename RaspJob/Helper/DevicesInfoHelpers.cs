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
    public class DevicesInfoHelpers
    {

        public RequestHelper requestHelper = new RequestHelper();
        /// <summary>
        /// 由資料庫中取得使用者擁有的藍芽裝置資訊
        /// </summary>
        /// <returns></returns>
        public List<UserDevicesInfo> GetAllUserDevicesInfo()
        {
            List<UserDevicesInfo> result;
            string resultJsonString = requestHelper.DoGetRequest(App.Config.GetAllUserDevicesInfoUrl);
            result = JsonStringToInfoList<UserDevicesInfo>(resultJsonString);

            return result;
        }

        /// <summary>
        /// 由資料庫中取得Beacon裝置資訊
        /// </summary>
        /// <returns></returns>
        public List<IotBeaconInfo> GetIotBeaconInfo()
        {
            List<IotBeaconInfo> result;
            string resultJsonString = requestHelper.DoGetRequest(App.Config.GetIotBeaconInfoUrl);
            result = JsonStringToInfoList<IotBeaconInfo>(resultJsonString);

            return result;
        }




        /// <summary> 
        /// 將DataTable轉成List
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<T> JsonStringToInfoList<T>(string jsonString)
        {
            List<T> list = JsonConvert.DeserializeObject<List<T>>(jsonString);

            return list;
        }
    }
}
