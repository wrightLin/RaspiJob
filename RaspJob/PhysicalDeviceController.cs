using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Physical Device Communcation 
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using System.Threading;

namespace RaspJob
{
    public class PhysicalDeviceController
    {
        // 控制繼電器GPIO pin
        public GpioPin GateControllPin;
        public GpioPin PIRControllPin;
        //public GpioPin PIRControllPin2;
        public GpioPin LEDControllPin;


        public PhysicalDeviceController()
        {
            // 設定 GPIO
            var gpio = GpioController.GetDefault();
            GateControllPin = gpio.OpenPin(App.Config.GatePin);
            GateControllPin.SetDriveMode(GpioPinDriveMode.Output);
            GateControllPin.Write(GpioPinValue.Low);

            // 設定 紅外線 Sensor
            PIRControllPin = GpioController.GetDefault().OpenPin(App.Config.PIRSensorPin);
            PIRControllPin.SetDriveMode(GpioPinDriveMode.Input);
            //PIRControllPin2 = GpioController.GetDefault().OpenPin(Constant.PIRSensorPin2);
            //PIRControllPin2.SetDriveMode(GpioPinDriveMode.Input);



            // 設定 紅外線 Sensor 變化時點亮LED
            PIRControllPin.ValueChanged += PIRDetected_LightLED;
            //PIRControllPin2.ValueChanged += PIRDetected_LightLED;
            PIRControllPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            //PIRControllPin2.DebounceTimeout = TimeSpan.FromMilliseconds(50);


            // 設定 LED GPIO
            LEDControllPin = GpioController.GetDefault().OpenPin(App.Config.LEDPin);
            LEDControllPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        public void OpenGate()
        {
            GateControllPin.Write(GpioPinValue.High);
            // Delay 
            using (EventWaitHandle tmpEvent = new ManualResetEvent(false))
            {
                tmpEvent.WaitOne(TimeSpan.FromSeconds(App.Config.OpenDoorSeconds));
            }
            GateControllPin.Write(GpioPinValue.Low);
            return;
        }


        public void LightLED()
        {
            LEDControllPin.Write(GpioPinValue.High);
            return;

        }
        public void CloseLED()
        {
            LEDControllPin.Write(GpioPinValue.Low);
            return;

        }


        public bool IsPIRSensorDetected()
        {
            if (
                    PIRControllPin.Read() == GpioPinValue.High
                    //||
                    //PIRControllPin2.Read() == GpioPinValue.High
                )
                return true;

            return false;


        }


        private void PIRDetected_LightLED(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            if (e.Edge == GpioPinEdge.RisingEdge)
                LightLED();
            else 
                CloseLED();

            return;
        }

    }
}
