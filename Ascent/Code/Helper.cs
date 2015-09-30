using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Ascent.Code
{
    public class Helper
    {
        static MessageDialog md = new MessageDialog("", "");
        public static bool IsFeedbackDialogOpen = false;
        public async static void ShowMessage(string content, string title = "")
        {
            md.Title = title;
            md.Content = content;
            await md.ShowAsync();
        }
    }
}
