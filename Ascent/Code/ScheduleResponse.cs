using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ascent.Code
{
    class ScheduleResponse : ResponseBase
    {
        public object Response { get; private set; }
        
        public ScheduleResponse(string urlContent) 
            : base(urlContent)
        {

            if (Success)
            {
                if (InnerResponse.Type == Newtonsoft.Json.Linq.JTokenType.String)
                {
                    Response = (string)InnerResponse;
                }
                else if (InnerResponse.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                {
                    Response = new List<ScheduleItem>();
                    JArray ar = (JArray)InnerResponse;

                    foreach (JObject item in ar)
                    {
                        (Response as List<ScheduleItem>).Add(new ScheduleItem()
                            {
                                ID = int.Parse((string)item["sched_id"]),
                                Date = DateTime.Parse((string)item["sched_date"]),
                                Time = ((string)item["sched_time"]).Trim(),
                                Activity = ((string)item["sched_activity"]).Trim(),
                                Faculty = ((string)item["sched_faculty"]).Trim(),
                                RegionID = int.Parse((string)item["sched_region"])
                            });
                    }
                }

            }
        }
    }
}
