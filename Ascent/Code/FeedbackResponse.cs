using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ascent.Code
{
    class FeedbackResponse : ResponseBase
    {
        public object Response { get; private set; }

        public FeedbackResponse(string urlContent)
            : base(urlContent)
        {
            if (Success)
                Response = (string)InnerResponse;
        }
    }
}
