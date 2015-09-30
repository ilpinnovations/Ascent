using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Ascent.Code
{
    abstract class ResponseBase
    {
        public string ResponseType { get; protected set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        protected JToken InnerResponse { get; set; }

        public ResponseBase(string urlContent)
        {
            JObject root = JObject.Parse(urlContent);
            ResponseType = (string)root["response_type"];
            Success = (bool)root["success"];

            if (!Success)
            {
                ErrorMessage = (string)root["error_msg"];
            }
            else
            {
                InnerResponse = root["response"];
            }
        }
    }
}
