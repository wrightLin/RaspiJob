using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Bluetooth.Advertisement;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using Windows.Storage;
using System.Diagnostics;

namespace RaspJob.Helper
{
    public class BeaconSimulator
    {
        //using Windows.Devices.Bluetooth.Advertisement;
        private BluetoothLEAdvertisementPublisher _blePublisher;


        public BeaconSimulator()
        {
            _blePublisher = new BluetoothLEAdvertisementPublisher();


        }




        public async void TurnOnBeaconSimulator()
        {
            //先讀取 beaconData 設定的 Beacon 資訊
            var beaconData = GetBeaconData();
            // 開始廣播 iBeacon 的廣告訊息
            PublishiBeacon(beaconData);

        }


        private void PublishiBeacon(byte[] dataArray)
        {
            var manufactureData = new BluetoothLEManufacturerData();
            //0x004C	Apple, Inc.
            manufactureData.CompanyId = 0x004c;
            manufactureData.Data = dataArray.AsBuffer();
            _blePublisher.Advertisement.ManufacturerData.Add(manufactureData);
            //開始發佈
            _blePublisher.Start();
        }



        /// <summary>
        /// 取得 beacon 的資訊
        /// </summary>
        /// <returns></returns>
        private byte[] GetBeaconData()
        {

            StringBuilder beaconData = new StringBuilder();
            beaconData.Append(App.Config.manufacturerId);
            beaconData.Append((App.Config.uuid).Replace("-", string.Empty));
            beaconData.Append(App.Config.major);
            beaconData.Append(App.Config.minor);
            beaconData.Append(App.Config.txPower);
            string beaconDataStr = beaconData.ToString();
            var result = Enumerable.Range(0, beaconDataStr.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(beaconDataStr.Substring(x, 2), 16))
                .ToArray();

            return result;
        }

       

    }
}
