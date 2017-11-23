using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace RaspJob
{
    public class RaspJobConfig
    {
        // GPIO Setting
        public  int GatePin ;
        public  int PIRSensorPin ;
        public  int PIRSensorPin2 ;
        public  int LEDPin ;

        // 門禁相關 
        public  string OpenDoorMessage ;
        public  string ClickOpenDoorMessage;
        public  int OpenDoorSeconds ;
        public  int OpenDoorIntervalSeconds ;

        //Log
        public  string LogDateTimeFormat ;

        // Beacon Detector
        public  short ReceiveBeaconRSSIThreshold;

        // IOT Device Info && IOT Beacon Info
        public  string GetAllUserDevicesInfoUrl ;
        public  string GetIotBeaconInfoUrl ;
        public  string BeaconWebApiAuthHeadKey;
        public  string BeaconWebApiAuthHeadValue;

        // Log Url
        public  string PostInfoMessage2LogUrl ;
        public  string PostErrorMessage2LogUrl;


        // BeaconSimulator
        public string manufacturerId;
        public string uuid;
        public string major;
        public string minor;
        public string txPower;


        // Iot Hub
        public string DeviceName;
        public string DeviceKey;
        public string IOTHubUri;





        public RaspJobConfig()
        {
            string fileContent = ReadConfigFromFile();
            dynamic config = JsonConvert.DeserializeObject(fileContent);

            GatePin = (int)config.GatePin;
            PIRSensorPin = (int)config.PIRSensorPin;
            PIRSensorPin2 = (int)config.PIRSensorPin2;
            LEDPin = (int)config.LEDPin;
            OpenDoorMessage = (string)config.OpenDoorMessage;
            ClickOpenDoorMessage = (string)config.ClickOpenDoorMessage;
            OpenDoorSeconds = (int)config.OpenDoorSeconds;
            OpenDoorIntervalSeconds = (int)config.OpenDoorIntervalSeconds;
            LogDateTimeFormat = (string)config.LogDateTimeFormat;
            ReceiveBeaconRSSIThreshold = (short)config.ReceiveBeaconRSSIThreshold;
            GetAllUserDevicesInfoUrl = (string)config.GetAllUserDevicesInfoUrl;
            GetIotBeaconInfoUrl = (string)config.GetIotBeaconInfoUrl;
            BeaconWebApiAuthHeadKey = (string)config.BeaconWebApiAuthHeadKey;
            BeaconWebApiAuthHeadValue = (string)config.BeaconWebApiAuthHeadValue;
            PostInfoMessage2LogUrl = (string)config.PostInfoMessage2LogUrl;
            PostErrorMessage2LogUrl = (string)config.PostErrorMessage2LogUrl;
            manufacturerId = (string)config.manufacturerId;
            uuid = (string)config.uuid;
            major = (string)config.major;
            minor = (string)config.minor;
            txPower = (string)config.txPower;
            DeviceName = (string)config.DeviceName;
            DeviceKey = (string)config.DeviceKey;
            IOTHubUri = (string)config.IOTHubUri;

        }



        /// <summary>
        /// 讀取 Config.json 的內容
        /// </summary>
        /// <returns></returns>
        private string ReadConfigFromFile()
        {
            var fileName = @"Config.json";
            StorageFolder folder;
            StorageFile file;
            try
            {
                //先在 LocalFolder 裡找
                folder = ApplicationData.Current.LocalFolder;
                file = folder.GetFileAsync(fileName).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                //找不到檔案，所以取預設的地方
                folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                file = folder.GetFileAsync(fileName).GetAwaiter().GetResult();
            }
            var fileContent = FileIO.ReadTextAsync(file).GetAwaiter().GetResult();
            return fileContent;
        }

    }
}
