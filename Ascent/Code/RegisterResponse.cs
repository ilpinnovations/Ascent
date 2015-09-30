using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ascent.Code
{
    class RegisterResponse : ResponseBase
    {
        public object Response { get; private set; }

        public RegisterResponse(string urlContent)
            : base(urlContent)
        {
            if (Success)
                Response = (string)InnerResponse;
        }
    }
}
