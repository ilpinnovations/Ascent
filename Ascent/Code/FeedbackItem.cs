using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ascent.Code
{
    public class FeedbackItem
    {
        [JsonProperty("ScheduleID")]
        public int ScheduleID { get; set; }

        [JsonProperty("EmployeeID")]
        public int EmployeeID { get; set; }

        [JsonProperty("Rating")]
        public int Rating { get; set; }

        [JsonProperty("Comments")]
        public string Comments { get; set; }

        public FeedbackItem()
        {
            ScheduleID = EmployeeID = -1;
            Rating = 0; Comments = "";
        }
        public FeedbackItem(int scheduleId, int employeeId, int rating, string comments)
        {
            ScheduleID = scheduleId;
            EmployeeID = employeeId;
            Rating = rating;
            Comments = comments;
        }
    }
}
