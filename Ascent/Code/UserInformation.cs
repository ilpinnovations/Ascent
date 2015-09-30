using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Ascent.Code
{
    class UserInformation
    {
        static string _fullName, _emailId;
        static int _employeeId;
        static int _regionId;
        static string _regionName;
        
        public static int EmployeeId { get { return _employeeId; } set { _employeeId = value; } }
        public static string FullName { get { return _fullName; } set { _fullName = value; } }
        public static int RegionId { get { return _regionId; } set { _regionId = value; } }
        public static string RegionName { get { return _regionName; } set { _regionName = value; } }
        public static string EmailId { get { return _emailId; } set { _emailId = value; } }
        public static void UpdateUserInformation(int employeeId,
            string fullName, string emailId, int regionId, string regionName)
        {
            EmployeeId = employeeId;
            FullName = fullName;
            EmailId = emailId;
            RegionId = regionId;
            RegionName = regionName;
            //Save to disk
            SaveUserInformation();
        }
        public static bool GetUserInformation()
        {
            bool IsFirstLaunch = false;

            var localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("NOT_FIRST_LAUNCH"))
            {
                EmployeeId = (int)localSettings.Values["KEY_EMP_ID"];
                FullName = (string)localSettings.Values["KEY_FULL_NAME"];
                EmailId = (string)localSettings.Values["KEY_EMAIL_ID"];
                RegionId = (int)localSettings.Values["KEY_REGION_ID"];
                RegionName = (string)localSettings.Values["KEY_REGION_NAME"];
            }
            else
            {
                FullName = EmailId = string.Empty;
                EmployeeId = -1;
                RegionId = 0; RegionName = "None";
                IsFirstLaunch = true;
            }

            return IsFirstLaunch;
        }
        public static void ClearUserInformation()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values.Clear();
        }
        public static void SaveUserInformation()
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            localSettings.Values["KEY_EMP_ID"] = EmployeeId;
            localSettings.Values["KEY_FULL_NAME"] = FullName;
            localSettings.Values["KEY_EMAIL_ID"] = EmailId;
            localSettings.Values["KEY_REGION_ID"] = RegionId;
            localSettings.Values["KEY_REGION_NAME"] = RegionName;

            #region MiscSaves
            //WARNING! App depends on the following key, do not delete!
            localSettings.Values["NOT_FIRST_LAUNCH"] = "Developed by: Milind Gour";
            #endregion        
        }
    }
}
