using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspJob.Models
{
    public class LogInfo
    {
        public string UserID { get; set; }
        public string DeviceName { get; set; }
        public string DeviceAction { get; set; }
        public string LogMessage { get; set; }
        public string DeviceDateTime { get; set; }
    }
}
