using Ascent.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Ascent.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SchedulePage : Page
    {
        public SchedulePage()
        {
            this.InitializeComponent();

            feedControl.FeedbackCancelled += feedControl_FeedbackCancelled;
            feedControl.FeedbackReceived += feedControl_FeedbackReceived;

            feedControl.InitializeDialog();
        }

        void SetIsEnableControls(bool isEnabled)
        {
            dateControl.IsEnabled =
                btnGet.IsEnabled =
                listSchedule.IsEnabled = isEnabled;
        }
        void SetProgressBarVisibility (bool visible)
        {
            if (visible) progressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        async void feedControl_FeedbackReceived(FeedbackItem feedbackObj)
        {
            // Received feedback from the control
            try
            {
                SetIsEnableControls(true);
                SetProgressBarVisibility(true);
                var response = await Ascent.Code.Ascent.GiveFeedback(feedbackObj);
                if (!response.Success)
                {
                    Helper.ShowMessage(response.ErrorMessage, "Information");
                }
                else
                {
                    Helper.ShowMessage("Thank you for you valuable feedback", "Information");
                }

            }
            catch (Exception)
            {
                Helper.ShowMessage("Please make sure you have an active internet connection to give feedback. Please try again later.", "Error");
            }
            finally
            {
                SetProgressBarVisibility(false);
            }
        }

        void feedControl_FeedbackCancelled(FeedbackItem feedbackObj)
        {
            // Cancelled the feedback control
            SetIsEnableControls(true);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadScheduleInUI();
        }

        private void listSchedule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listSchedule.SelectedIndex == -1) return;

            ScheduleItem selectedItem = listSchedule.SelectedItem as ScheduleItem;

#if DEBUG
            System.Diagnostics.Debug.WriteLine("Selected Schedule: " + selectedItem.Activity);
#endif

            if (!string.IsNullOrEmpty(selectedItem.Faculty))
            {
                //Check for Future dates
                DateTime scheduleDate = selectedItem.Date.Date;
                DateTime today = DateTime.Now.Date;
                if (today < scheduleDate)
                {
                    Helper.ShowMessage("You cannot give feedback to future schedules", "Error");
                }
                else
                {
                    SetIsEnableControls(false);
                    Helper.IsFeedbackDialogOpen = true;
                    feedControl.GetFeedback(selectedItem);
                }
            }

            listSchedule.SelectedIndex = -1;
        }
        private async void LoadScheduleInUI(string date)
        {
            var list = await Ascent.Code.Ascent.GetSchedule(date);
            listSchedule.DataContext = null;
            listSchedule.DataContext = list;
        }
        private async void LoadScheduleInUI(bool forceServer = false)
        {
            SetProgressBarVisibility(true);
            SetIsEnableControls(false);
            try
            {
                DateTimeOffset dateValue = dateControl.Date;
                string date = string.Format("{0}-{1}-{2}", dateValue.Year, dateValue.Month, dateValue.Day);

                var list = await Ascent.Code.Ascent.GetSchedule(date, forceServer);
                listSchedule.DataContext = null;
                listSchedule.DataContext = list;
            }
            catch (InvalidDataException ex)
            {
                listSchedule.DataContext = null;
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Exception Caught: " + ex.Message);
#endif

                Helper.ShowMessage(ex.Message, "Error");
            }
            catch (Exception)
            {
                listSchedule.DataContext = null;
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Exception Caught: No Internet");
#endif

                Helper.ShowMessage("App needs an active internet connection to fetch the schedule for the first time.", "Error");
            }
            finally
            {
                SetProgressBarVisibility(false);
                SetIsEnableControls(true);
            }
        }

        private void dateControl_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            LoadScheduleInUI();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadScheduleInUI(true);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Help button clicked
            string title = "Information";
            string content = "Click on a session to give feedback. You can check schedule for other day by selecting the appropriate "
                + "date. The schedule will get downloaded and stored on the device for offline view."
                + "\n\nCopyright© 2015 ILP Innovations";

            Helper.ShowMessage(content, title);
        }
    }
}
