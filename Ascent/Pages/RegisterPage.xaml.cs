using Ascent.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class RegisterPage : Page
    {
        string fullName, emailId, regionName;
        int empId;
        int regionId;

        public RegisterPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadUserInformation();
        }

        private void ShowProgressBar(bool show)
        {
            if (show) progressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        private async void tbSubmit_Click(object sender, RoutedEventArgs e)
        {
            // Submit button clicked
                if (await ValidateData())
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Passed the validation on the data");
#endif
                RegisterResponse response = null;
                try
                {
                    response = await Ascent.Code.Ascent.RegisterUser(new RegisterItem(empId, fullName, regionId, emailId));
                }
                catch (Exception)
                {
                    Helper.ShowMessage("Please make sure you have an active internet connection", "Error");
                    ShowProgressBar(false);
                    return;
                }
                finally
                {
                    ShowProgressBar(false);
                }
//#if !DEBUG
                //Navigate
                if (response.Success)
                {
                    // Update with new user information
                    UpdateUserInformation();
                    Frame.Navigate(typeof(SchedulePage));
                    Frame.BackStack.Clear();
                }
                else
                {
                    Helper.ShowMessage((string)response.Response, "Error");
                }
//#else
//                // For Debugging this page
//                if (response.Success)
//                    Helper.ShowMessage((string)response.Response, "Success");
//                else
//                    Helper.ShowMessage(response.ErrorMessage, "Failure");
//#endif
            }
        }

        /// <summary>
        /// Returns true on success, exception on failure
        /// </summary>
        /// <returns>true on success</returns>
        async Task<bool> ValidateData()
        {
            List<string> invalids = new List<string>();
            bool isValid = true;

            if (int.TryParse(tbEmployeeId.Text, out empId) == false)
                empId = -1;

            fullName = tbEmployeeName.Text;
            emailId = tbEmailId.Text;

            if (cbRegion.SelectedIndex == -1)
                isValid = false;
            else
                regionId = (cbRegion.SelectedItem as RegionItem).ID;

            if (empId <= 0)
            {
                isValid = false;
                invalids.Add("Employee ID");
            }
            if (string.IsNullOrEmpty(fullName))
            {
                isValid = false;
                invalids.Add("Employee Name");
            }
            if (regionId == -1)
            {
                isValid = false;
                invalids.Add("Region");
            }

            if (string.IsNullOrEmpty(emailId) ||
                !emailId.EndsWith("@tcs.com"))
            {
                isValid = false;
                invalids.Add("Email Id");
            }

            if (!isValid)
            {
                string msg = "The following fields are invalid. Please correct them to continue:\n";
                msg += string.Join(", ", invalids);
                string title = "Error";

                Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog(msg, title);
                await md.ShowAsync();
                return false;
            }

            return true;
        }
        async void LoadUserInformation()
        {
            try
            {
                cbRegion.DataContext = await Ascent.Code.Ascent.GetRegionList();
                tbEmployeeId.Text = string.Empty;
                tbEmployeeName.Text = string.Empty;
                cbRegion.SelectedIndex = 0;
                tbEmailId.Text = string.Empty;
            }
            catch (Exception)
            {
                ConfirmExit();
            }
        }
        async void ConfirmExit()
        {
            await (new MessageDialog("Make sure you have an active internet connection. The app will now exit.", "Information")).ShowAsync();
            App.Current.Exit();

        }
        void UpdateUserInformation()
        {
            regionName = (cbRegion.SelectedItem as RegionItem).Name;
            UserInformation.UpdateUserInformation(empId, fullName, emailId, regionId, regionName);
        }
    }
}
