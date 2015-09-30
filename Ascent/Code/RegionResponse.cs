using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ascent.Code
{
    class RegionResponse : ResponseBase
    {
        public object Response { get; private set; }

        public RegionResponse(string urlContent) 
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
                    Response = new List<RegionItem>();
                    JArray ar = (JArray)InnerResponse;

                    foreach (JObject item in ar)
                    {
                        (Response as List<RegionItem>).Add(new RegionItem()
                            {
                                ID = int.Parse((string)item["region_id"]),
                                Name = (string)item["region_name"]
                            });
                    }
                }

            }
        }
    }
}
