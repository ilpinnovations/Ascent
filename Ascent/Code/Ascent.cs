using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace Ascent.Code
{
    class Ascent
    {
        static Random randomGenerator = new Random();
        //const string BASE_URL = "http://localhost/ascent";
        const string BASE_URL = "http://theinspirer.in/ascent";
        const string SCHEDULE_FILENAME_PREFIX = "iD351r3";
        public const string OFFLINE_FEEDBACK_PREFIX = "OFFLINE_FEEDBACK";

        #region CLIENT STRINGS
        public const string CLIENT_OFFLINE_JSON = "{\"response_type\":\"CLIENT\",\"success\":false,\"error_msg\":\"You are offline. The given feedback has been stored offline and will be uploaded once you are online.\"}";
        public const string CLIENT_OFFLINE_NONEW_JSON = "{\"response_type\":\"CLIENT\",\"success\":false,\"error_msg\":\"You are offline. There is already a copy of feedback stored on the device which will be uploaded once you go online.\"}";
        #endregion

        public async static Task< List<RegionItem> > GetRegionList()
        {
            string urlContent = await GetURLContentAsString("?action=getRegion");
            return ParseRegion(urlContent);
        }
        public async static Task<RegisterResponse> RegisterUser(RegisterItem userInfo)
        {
            string url = string.Format("?action=register&empId={0}&empName={1}&regionId={2}&emailId={3}",
                userInfo.ID, userInfo.Name, userInfo.RegionID, userInfo.Email);
            string urlContent = await GetURLContentAsString(url);

            return ParseRegisterResponse(urlContent);
        }
        public async static Task<FeedbackResponse> GiveFeedback(FeedbackItem feedInfo)
        {
            string url = string.Format("?action=setFeedback&schedId={0}&empId={1}&rating={2}&comments={3}",
                feedInfo.ScheduleID, feedInfo.EmployeeID, feedInfo.Rating, feedInfo.Comments);
            string urlContent = await GetURLContentAsString(url);

            if (string.IsNullOrEmpty(urlContent))
            {
                // User's phone went offline before comment upload
                var localSettings = ApplicationData.Current.LocalSettings;
                string settingName = string.Format("{0}_{1}_{2}", OFFLINE_FEEDBACK_PREFIX, feedInfo.ScheduleID, feedInfo.EmployeeID);
                if (!localSettings.Values.Keys.Contains(settingName))
                {
                    string serializedString = JsonConvert.SerializeObject(feedInfo);
#if DEBUG 
                    System.Diagnostics.Debug.WriteLine("Serializing into setting: {0}\nValue: {1}\n", settingName, serializedString);
#endif
                    localSettings.Values[settingName] = serializedString;

                    return new FeedbackResponse(CLIENT_OFFLINE_JSON);
                }
                else
                {
                    return new FeedbackResponse(CLIENT_OFFLINE_NONEW_JSON);
                }
            }

            return ParseFeedbackResponse(urlContent);
        }

        public async static void TrySendingOfflineFeedbacks()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            foreach (var item in localSettings.Values.Keys)
            {
                if (item.StartsWith(OFFLINE_FEEDBACK_PREFIX))
                {
                    string itemJson = (string)localSettings.Values[item];
                    FeedbackItem feedItem = JsonConvert.DeserializeObject<FeedbackItem>(itemJson);
                    var response = await GiveFeedback(feedItem);
                    
                    //response.ResponseType == CLIENT, when user is still offline, don't delete the setting
                    if (response.ResponseType != "CLIENT")
                    {
                        localSettings.Values.Remove(item);
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("[OFFLINE FEEDBACK]: id={0} UPLOADED, Result = {1}", feedItem.ScheduleID, response.Response);
#endif
                    } 
                    else
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("[OFFLINE FEEDBACK]: id={0} Failed because user is offline", feedItem.ScheduleID);
#endif
                    }
                }
            }
        }
        private static FeedbackResponse ParseFeedbackResponse(string urlContent)
        {
            FeedbackResponse response = new FeedbackResponse(urlContent);
            return response;
        }


        private static RegisterResponse ParseRegisterResponse(string urlContent)
        {
            RegisterResponse response = new RegisterResponse(urlContent);
            return response;
        }

        private static List<RegionItem> ParseRegion(string urlContent)
        {
            RegionResponse regionResponse = new RegionResponse(urlContent);
            if (regionResponse.Success)
            {
                return regionResponse.Response as List<RegionItem>;
            }

            throw new InvalidDataException(regionResponse.ErrorMessage);
        }

        public async static Task<List<ScheduleItem> > GetSchedule(string date, bool force = false)
        {
            List<ScheduleItem> scheduleList;
            int regionId = UserInformation.RegionId;

            //if available locally, read that
            //otherwise, fetch from server
            string filename = string.Format("{0}_{1}{2}.json", SCHEDULE_FILENAME_PREFIX, regionId, date);
            string urlContent = await ReadContentsFromDisk(filename);

            if (urlContent.Length == 0 || force)
            {
                string url = string.Format("?action=getSchedule&date={0}&regionId={1}", date, regionId);
                urlContent = await GetURLContentAsString(url);
#if DEBUG
                System.Diagnostics.Debug.WriteLine("[SCHEDULE] Disk Read FAILURE, {0} does not exist.", filename);
#endif

                scheduleList = ParseSchedule(urlContent);
                //Write file to disk
                if (scheduleList.Count > 0)
                {
                    SaveContentsToDisk(filename, urlContent);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("[SCHEDULE] File {0} written to disk.", filename);
#endif
                }
            }
            else
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("[SCHEDULE] Disk Read SUCCESS");
#endif
                scheduleList = ParseSchedule(urlContent);
            }

            return scheduleList;
        }

        private static List<ScheduleItem> ParseSchedule(string urlContent)
        {
            ScheduleResponse schedResponse = new ScheduleResponse(urlContent);
            if (schedResponse.Success)
            {
                return schedResponse.Response as List<ScheduleItem>;
            }

            throw new InvalidDataException(schedResponse.ErrorMessage);
        }

        private async static Task<string> GetURLContentAsString(string relativeURL)
        {
            // GET request for the update
            HttpClient client = new HttpClient();
            string url = BASE_URL + relativeURL + "&iRandom=" + randomGenerator.Next(1000, 10000);
            var response = await client.GetAsync(url);

#if DEBUG
            System.Diagnostics.Debug.WriteLine("*******\nURL\t\t\t\t: " + url + "\nPayload length\t: " + response.Content.Headers.ContentLength + " byte(s)\n*******");
#endif

            return response.Content.ReadAsStringAsync().Result;
        }
        private static async Task<string> ReadContentsFromDisk(string filename)
        {
            string content = "";

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

#if DEBUG
            var files = await localFolder.GetFilesAsync();

            System.Diagnostics.Debug.WriteLine("-------------------------------------------------------------------");
            System.Diagnostics.Debug.WriteLine("Following {0} files exist in the storage:", files.Count);
            ulong totalFileSize = 0;
            foreach (var file in files)
            {
                var properties = await file.GetBasicPropertiesAsync();
                totalFileSize += properties.Size;
                bool isJson = Path.GetExtension(file.Name) == ".json";
                System.Diagnostics.Debug.WriteLine(file.Name + ", File Size = " + properties.Size + " bytes");
            }
            System.Diagnostics.Debug.WriteLine("Total Size of Local Folder = {0} bytes", totalFileSize);
            System.Diagnostics.Debug.WriteLine("-------------------------------------------------------------------");
#endif

            try
            {
                StorageFile contactsFile = await localFolder.GetFileAsync(filename);
                using (var stream = await contactsFile.OpenStreamForReadAsync())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        content = await reader.ReadToEndAsync();
                    }
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                return "";
            }

            return content;
        }
        private static async void SaveContentsToDisk(string filename, string contents)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile contactsFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (var stream = await contactsFile.OpenStreamForWriteAsync())
            {
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(contents);
                }
            }
        }

    }
}
