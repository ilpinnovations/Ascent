using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ascent.Code
{
    public class ScheduleItem
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Activity { get; set;}
        public string Faculty { get; set; }
        public int RegionID { get; set; }

        public ScheduleItem()
        {
            ID = -1;
            Date = DateTime.Now;
            Time = "no value";
            Activity = "none";
            Faculty = "none";
            RegionID = 1;
        }
    }
}
