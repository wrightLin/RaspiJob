//using PublishingBeacon;
using System;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Beacons;
using System.Collections.Generic;
using RaspJob;
using RaspJob.Models;

public class BLEDeviceDetector
{
    private DispatcherTimer timer;
    public BluetoothLEAdvertisementWatcher watcher;
    public BleDeviceInfo bleDeviceInfo;
    public delegate void AfterReceivedAction();
    public AfterReceivedAction afterReceivedAction;

    public BLEDeviceDetector()
    {
        watcher = new BluetoothLEAdvertisementWatcher();
        // Monitor all iBeacons advertisment
        watcher.AdvertisementFilter.Advertisement.iBeaconSetAdvertisement(new iBeaconData());
        watcher.Received += OnAdvertisementReceived;
    }



    /// <summary>
    /// 監聽到藍芽裝置時之觸發事件
    /// </summary>
    /// <param name="watcher"></param>
    /// <param name="eventArgs"></param>
    private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
    {
        bleDeviceInfo = new BleDeviceInfo();

        // Get iBeacon specific data
        bleDeviceInfo.BeaconData = eventArgs.Advertisement.iBeaconParseAdvertisement(eventArgs.RawSignalStrengthInDBm);

        // The received signal strength indicator (RSSI)
        bleDeviceInfo.rssi = eventArgs.RawSignalStrengthInDBm;

        // Get Device BluetoothAddress (To HEX)
        bleDeviceInfo.bluetoothAddress = eventArgs.BluetoothAddress.ToString("X");


        afterReceivedAction();
        return;
    }





}