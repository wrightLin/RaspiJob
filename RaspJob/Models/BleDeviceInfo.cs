using Beacons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspJob.Models
{
    public class BleDeviceInfo
    {
        public iBeaconData BeaconData { get; set; }
        public Int16 rssi { get; set; }
        public string bluetoothAddress { get; set; }

        public BleDeviceInfo()
        {
            BeaconData = new iBeaconData();
        }
    }
}
