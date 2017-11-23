using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspJob.Models
{
    public class IotBeaconInfo
    {
        public string BEACON_ID { get; set; }

        public string UUID { get; set; }

        public string MAJOR_ID { get; set; }

        public string MINOR_ID { get; set; }

        public string BEACON_TYPE { get; set; }

        public double TX_POWER { get; set; }

        public string IS_ENABLE { get; set; }

        public string EXTEND_VALUE { get; set; }

        public string LOCATION { get; set; }

        public string DESCRIPTION { get; set; }

        public string CATEGORY_ID { get; set; }

        public DateTime CRE_DTE { get; set; }

    }
}
