using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YelpApp_v1
{
    public class Business
    {
        public string name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public double distance { get; set; }
        public double stars { get; set; }
        public int tip_count { get; set; }
        public int checkin_count { get; set; }
        public string bid { get; set; }
    }
}
