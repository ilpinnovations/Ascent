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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Ascent.Design
{
    public delegate void FeedbackHandler(FeedbackItem feedbackObj);
    public sealed partial class FeedbackControl : UserControl
    {
        enum Mode { GiveFeedback, ShowRating }
        private bool _isOpen = false;
        public event FeedbackHandler FeedbackReceived;
        public event FeedbackHandler FeedbackCancelled;

        ScheduleItem _scheduleObject;

        private void OnFeedbackCancelled(FeedbackItem o)
        {
            if (FeedbackCancelled != null)
                FeedbackCancelled(o);
        }
        private void OnFeedbackReceived(FeedbackItem o)
        {
            if (FeedbackReceived != null)
                FeedbackReceived(o);
        }
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                if (_isOpen != value)
                {
                    if (_isOpen)
                    {
                        Feedback_Exit.Begin();
                        _isOpen = value;
                    }
                    else
                    {
                        ResetFields();
                        this.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        tbComments.IsEnabled = true;
                        Feedback_Entry.Begin();
                        _isOpen = value;
                    }
                }
            }
        }
        public void InitializeDialog()
        {
            ResetFields();
            Feedback_Exit.Begin();
            _isOpen = false;
            RequestedTheme = ElementTheme.Dark;
        }
        private void ResetFields()
        {
            tbComments.Text = string.Empty;
            rating.Value = 0;
        }
        public FeedbackControl()
        {
            this.InitializeComponent();
            IsOpen = false;
            Feedback_Exit.Completed += delegate
            {
                tbComments.IsEnabled = false; 
                this.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            };
        }
        public void GetFeedback(ScheduleItem scheduleItem)
        {
            if (!IsOpen)
            {
                SetDialogMode(Mode.GiveFeedback);

                this._scheduleObject = scheduleItem;
                tbCourseName.Text = scheduleItem.Activity;
                tbFacultyName.Text = scheduleItem.Faculty;
                IsOpen = true;
            }
        }
        //public void ShowAvgRating(string facultyName, string courseName, AvgRatingObject avgRateObj)
        //{
        //    if (!IsOpen)
        //    {
        //        SetDialogMode(Mode.ShowRating);
        //        tbCourseName.Text = courseName;
        //        tbFacultyName.Text = facultyName;

        //        double avgRounded = Math.Round(avgRateObj.AvgRating, 2);
        //        tbRating.Text = avgRounded.ToString();
        //        double offsetValue = avgRounded / 5;
        //        tbFeedbacks.Text = avgRateObj.TotalComments.ToString();
                
        //        IsOpen = true;
        //        rating.Offset = offsetValue;

        //    }
        //}

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (rating.Value == 0)
            {
                await (new MessageDialog("Rating cannot be zero", "Error")).ShowAsync();
                return;
            }
            if (string.IsNullOrEmpty(tbComments.Text))
            {
                await (new MessageDialog("Comments cannot be blank", "Error")).ShowAsync();
                return;
            }

            //submit clicked
            FeedbackItem feedObject = new FeedbackItem(_scheduleObject.ID,
                UserInformation.EmployeeId, rating.Value, tbComments.Text);

            IsOpen = false;
            OnFeedbackReceived(feedObject);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //cancel clicked
            IsOpen = false;
            FeedbackItem feedObject = new FeedbackItem(_scheduleObject.ID, UserInformation.EmployeeId, 0, string.Empty);
            OnFeedbackCancelled(feedObject);
        }
        private void SetDialogMode(Mode mode)
        {
            switch (mode)
            {
                case Mode.GiveFeedback:
                    spComments.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    bSubmit.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    rating.IsEnabled = true;
                    bCancel.Content = "Cancel";
                    break;
                case Mode.ShowRating:
                    spComments.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    bSubmit.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    rating.IsEnabled = false;
                    bCancel.Content = "Close";
                    break;
                default:
                    break;
            }
        }
    }
}
