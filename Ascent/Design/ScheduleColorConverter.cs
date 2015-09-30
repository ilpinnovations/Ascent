using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Ascent.Design
{
    class ScheduleColorConverter : IValueConverter
    {
        const string LL = "FF";
        const string DD = "55";
        const string R = "#" + LL + DD + DD;
        const string G = "#" + DD + LL + DD;
        const string B = "#" + DD + DD + LL;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string incoming = ((string)value).Trim().ToLower();
            string outgoing = B;

            switch (incoming)
            {
                case "lunch":
                    outgoing = G;
                    break;
                case "break":
                    outgoing = R;
                   break;
                default:
                    break;
            }

            return outgoing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
