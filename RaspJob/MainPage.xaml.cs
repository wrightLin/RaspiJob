using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Azure.Devices.Client;
using System;
using System.Text;
using System.Threading;
using Windows.Devices.Bluetooth.Advertisement;
using RaspJob.Helper;
using RaspJob.Models;
using MetroLog;
using MetroLog.Targets;
using Windows.UI.Core;
using Windows.Networking.Connectivity;




//空白頁項目範本收錄在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RaspJob
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Init

        // MetroLog
        ILogger log;

        // Log By Api
        private LoggerHelper loggerHelper;
        private LogInfo logInfo = new LogInfo();
        private string userIdInQueue;
        private string beaconIdInQueue;

        // iot hub 相關
        public IotHubHelper iotHubHelper;

        // Beacon 模擬器
        public BeaconSimulator beaconSimulator;

        // Physical Controller 
        public PhysicalDeviceController phyDeviceController;

        // Beacon 偵測
        private BLEDeviceDetector detector;
        private DevicesInfoHelpers deviceInfoHelper;
        private List<UserDevicesInfo> userDevicesInfoList;



        private void Init()
        {
            // MetroLog 
            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new StreamingFileTarget());
            log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();

            // Log by Api
            //loggerHelper = new LoggerHelper();

            // Iot Hub Device Info
            iotHubHelper = new IotHubHelper();

            // Beacon 模擬器初始化
            beaconSimulator = new BeaconSimulator();
            beaconSimulator.TurnOnBeaconSimulator();

            // IOT User Devices Info
            deviceInfoHelper = new DevicesInfoHelpers();
            userDevicesInfoList = deviceInfoHelper.GetAllUserDevicesInfo();

            // Beacon 偵測初始化
            detector = new BLEDeviceDetector();
            detector.afterReceivedAction = new BLEDeviceDetector.AfterReceivedAction(AfterReceiveBleDevice);
            detector.watcher.Start();



#if (ARM)
            // Physical Controller 
            phyDeviceController = new PhysicalDeviceController();
#endif



            // 更改UI
            var task = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.IP_ADDRESS.Text = String.Format("Device Ip：{0}", GetLocalIp());

            });

        }

        #endregion

        public MainPage()
        {
            try
            {
                this.InitializeComponent();
                Init();

                var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    //持續查詢 Iot Hub Queue Message
                    ReceiveCloudToDeviceMessagesAsync();
                });

            }
            catch (Exception ex)
            {
                this.log.Fatal("MainPage Error：" + ex.ToString());
            }

        }

        #region IOT HUB Queue 
        /// <summary>
        /// 持續接收從Cloud過來的資料
        /// 1.由IOT HUB 接收Queue資料
        /// 2.收到Queue後，另起一Thread啟動門禁
        /// 3.若在啟動門禁狀態時收到Queue資料，不作動，直接取下queue。
        /// </summary>
        public async void ReceiveCloudToDeviceMessagesAsync()
        {

            Task openDoorTask = null;
            while (true)
            {
                try
                {
                    // Receive Message From IOT Hub
                    Message receivedMessage = await iotHubHelper.deviceClient.ReceiveAsync();
                    // 收到Queue Message 
                    if (receivedMessage != null)
                    {
                        string queueMessage = Encoding.UTF8.GetString(receivedMessage.GetBytes());

                        if (openDoorTask == null || openDoorTask.IsCompleted || openDoorTask.IsFaulted)
                        {
                            openDoorTask = new Task(() =>
                            {
                                AfterReceivingQueue(queueMessage);
                            });
                            openDoorTask.Start();
                        }

                        // 刪除Queue Message
                        await iotHubHelper.deviceClient.CompleteAsync(receivedMessage);
                    }
                }
                catch (Exception ex)
                {
                    // 建立Device Client
                    iotHubHelper.deviceClient =
                        DeviceClient.Create(iotHubHelper.iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(iotHubHelper.deviecName, iotHubHelper.deviceKey), TransportType.Amqp);
                    this.log.Fatal("exception:" + ex.ToString() + "DateTime:" + DateTime.Now.ToString(App.Config.LogDateTimeFormat));
                }
            }
        }

        /// <summary>
        /// 收到QueueMessge後做的事情
        /// </summary>
        public void AfterReceivingQueue(string queueMessage)
        {
            try
            {


#if (ARM)
                // 驗證
                if (OpenDoorValidationByMessage(queueMessage))
                {
                    phyDeviceController.OpenGate();

                    // 更改UI
                    var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        this.QueueMessage.Text = String.Format("Open Door By IoT Hub  --QueueMessage：{0}", queueMessage);

                    });



                    ///*Log By Api*/
                    //QueueParser(queueMessage);
                    //SaveLogByApi(this.deviecName, userIdInQueue, "OpenGate!",queueMessage, DateTime.Now.ToString(App.Config.LogDateTimeFormat));

                    // Delay
                    using (EventWaitHandle tmpEvent = new ManualResetEvent(false))
                    {
                        tmpEvent.WaitOne(TimeSpan.FromSeconds(App.Config.OpenDoorIntervalSeconds));
                    }
                }
#endif


            }
            catch (Exception ex)
            {
                this.log.Fatal("exception:" + ex.ToString() + "DateTime:" + DateTime.Now.ToString(App.Config.LogDateTimeFormat));
            }

            return;
        }

        /// <summary>
        /// 對QueueMessage 進行分析與驗證
        /// </summary>
        /// <param name="queueMessage"></param>
        /// <returns></returns>
        private bool OpenDoorValidationByMessage(string queueMessage)
        {
            try
            {
                if (
                    (queueMessage.Contains(App.Config.OpenDoorMessage)
                    && phyDeviceController.IsPIRSensorDetected())
                    ||
                    queueMessage.Contains(App.Config.ClickOpenDoorMessage)
                    )
                {
                    return true;
                }
                else if (queueMessage.Contains(App.Config.OpenDoorMessage))
                {
                    this.log.Info("ReceivingMessage But Not PIR Detected " + queueMessage + "DateTime:" + DateTime.Now.ToString(App.Config.LogDateTimeFormat));
                }

                return false;
            }
            catch (Exception ex)
            {
                this.log.Fatal("exception:" + ex.ToString() + "DateTime:" + DateTime.Now.ToString(App.Config.LogDateTimeFormat));
                throw;
            }
        }
        #endregion


        #region 頁面事件
        /// <summary>
        /// 檢測UWP是否活著
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsUWPAliveButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsUWPAliveButton.Content = "UWP_Alive!--" + DateTime.Now.ToString("hh:mm:ss");
        }
        #endregion


        #region BLEDetector



        /// <summary>
        /// 同時符合以下三種情況才開門
        /// 1.接收之 Beacon RSSI 高於門檻值 
        /// 2.紅外線動作感測器感測到使用者
        /// 3.掃描到的Device存於資料庫中
        /// </summary>
        /// <param name="beaconData"></param>
        /// <returns></returns>
        private bool OpenDoorValidationByDeviceInfo(BleDeviceInfo bleDeviceInfo)
        {
            string deviceKey = bleDeviceInfo.bluetoothAddress;
            int rssi = bleDeviceInfo.rssi;

            // 如果掃描到有beacon資料
            if (bleDeviceInfo.BeaconData.UUID != String.Empty)
            {
                deviceKey = bleDeviceInfo.BeaconData.UUID;
                rssi = bleDeviceInfo.BeaconData.Rssi;
            }



            // Device Bluetooth Address  From DB 
            var matchDevice = (
                                from x in userDevicesInfoList
                                where x.DEVICE_KEY.IndexOf(deviceKey, StringComparison.OrdinalIgnoreCase) >= 0 //忽略大小寫
                                && x.EXTEND_VALUE <=rssi //rssi 大於預設值
                                select x);

            if (matchDevice.Count() == 0)
                return false;

#if (ARM)
            // PIR Sensor
            if (!phyDeviceController.IsPIRSensorDetected())
                return false;
#endif



            return true;
        }


        /// <summary>
        /// 當BleDeeviceDetector 偵測到周遭裝置時
        /// </summary>
        private void AfterReceiveBleDevice()
        {
            if (OpenDoorValidationByDeviceInfo(detector.bleDeviceInfo))
            {
#if (ARM)
                phyDeviceController.OpenGate();
#endif

                // 更改UI
                var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    this.QueueMessage.Text = String.Format("Open Door By BleDetector--Device Key：{0}",
                        detector.bleDeviceInfo.BeaconData.UUID==String.Empty? detector.bleDeviceInfo.bluetoothAddress: detector.bleDeviceInfo.BeaconData.UUID); 

                });
                

                // Delay
                using (EventWaitHandle tmpEvent = new ManualResetEvent(false))
                {
                    tmpEvent.WaitOne(TimeSpan.FromSeconds(App.Config.OpenDoorIntervalSeconds));
                }
            }
            return;
        }


        #endregion


        #region Private Methods
        private void SaveLogByApi(string deviceName, string userId, string deviceAction, string logMessage, string deviceDatetime)
        {
            try
            {
                logInfo.DeviceName = deviceName;
                logInfo.DeviceAction = deviceAction;
                logInfo.LogMessage = logMessage;
                logInfo.UserID = userId;
                logInfo.DeviceDateTime = deviceDatetime;

                loggerHelper.PostUWPLogByApi(logInfo);
            }
            catch (Exception ex)
            {
                this.log.Info(" ※※※Exception※※※ " + ex + " ※※※DateTime※※※ :" + DateTime.Now.ToString(App.Config.LogDateTimeFormat));
                throw;
            }


        }

        private void QueueParser(string queueMessage)
        {
            try
            {
                char[] splitArray = { '：', '；' };
                userIdInQueue = queueMessage.Split(splitArray)[1];
                beaconIdInQueue = queueMessage.Split(splitArray)[3];
            }
            catch (Exception)
            {
                userIdInQueue = "No User ID In Queue";
                beaconIdInQueue = "No Beacon ID In Queue ";
            }

            return;
        }



        private string GetLocalIp()
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            if (icp?.NetworkAdapter == null) return null;
            var hostname =
                NetworkInformation.GetHostNames()
                    .SingleOrDefault(
                        hn =>
                            hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                            == icp.NetworkAdapter.NetworkAdapterId);

            // the ip address
            return hostname?.CanonicalName;
        }

        #endregion
    }
}
