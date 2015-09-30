using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ascent.Code
{
    class RegisterItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int RegionID { get; set; }
        public string Email { get; set; }

        public RegisterItem() { }
        public RegisterItem(int id, string name, int regionid, string email)
        {
            ID = id; Name = name; RegionID = regionid; Email = email;
        }
    }
}
