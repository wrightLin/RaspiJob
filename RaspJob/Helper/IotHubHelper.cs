using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Windows.Storage;
using System.Diagnostics;

namespace RaspJob.Helper
{
    public class IotHubHelper
    {
        // iot hub 相關
        public string deviecName;
        public string deviceKey;
        public string iotHubUri;
        public DeviceClient deviceClient;

        public IotHubHelper()
        {
            // 取得Iot Hub 相關資訊
            GetDeviceData();

            // 建立Device Client
            deviceClient =
                DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviecName, deviceKey), TransportType.Amqp);
        }





        /// <summary>
        /// 取得 beacon 的資訊
        /// </summary>
        /// <returns></returns>
        private void GetDeviceData()
        {
            deviecName = App.Config.DeviceName;
            deviceKey = App.Config.DeviceKey;
            iotHubUri = App.Config.IOTHubUri;

            return;
        }



    }
}
