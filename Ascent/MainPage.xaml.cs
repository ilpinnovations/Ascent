using Ascent.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Ascent
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            feedControl.InitializeDialog();
            feedControl.FeedbackCancelled += feedControl_FeedbackCancelled;
            feedControl.FeedbackReceived += feedControl_FeedbackReceived;
        }

        void feedControl_FeedbackReceived(Code.FeedbackItem feedbackObj)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Feedback Received: " + feedbackObj.Rating + " Stars, Comments = " + feedbackObj.Comments);
#endif
        }

        void feedControl_FeedbackCancelled(Code.FeedbackItem feedbackObj)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Feedback Cancelled: " + feedbackObj.EmployeeID + " for " + feedbackObj.ScheduleID);
#endif
        }

        private async void buttonClickHandler(object sender, RoutedEventArgs e)
        {
            string tag = (string)(sender as Button).Tag;
            switch (tag)
            {
                case "SCHEDULE":
                    Frame.Navigate(typeof(SchedulePage));
                    break; 
                case "REGION":
                    var y = await Ascent.Code.Ascent.GetRegionList();

                    string msg = "";
                    foreach (var item in y)
                    {
                        msg += string.Format("\n{0}, id={1}", item.Name, item.ID);
                    }
                    Ascent.Code.Helper.ShowMessage(msg, "Fetched regions");
                    break;
                case "REGISTER":
                    Frame.Navigate(typeof(RegisterPage));
                    break;
                case "FEEDBACK":
                    feedControl.GetFeedback(new Code.ScheduleItem() { Faculty = "Milind Gour", Activity = "Some good activity"});
                    break;
                default:
                    break;
            }
        }
    }
}
