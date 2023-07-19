using OneSignalApi.Api;
using OneSignalApi.Client;
using OneSignalApi.Model;
using System.Collections.Generic;
using babysittingIL.UserManagement;

namespace babysittingIL.PushMessaging
{
	class PushMessagingManager
	{
		public static void sendMessage(int fromID, int targetID, string content)
		{
			Configuration config = new Configuration();
			config.BasePath = "https://onesignal.com/api/v1";
			// Configure Bearer token for authorization: app_key
			config.AccessToken = "YzQ0MjYxMDAtOTY2Ni00MWIwLWEzZGUtYmU4ZjUwMzAwZDky";

			var apiInstance = new DefaultApi(config);
            var notification = new Notification(includeExternalUserIds: new List<string>() { targetID.ToString() }, largeIcon: user.GetUserByID(fromID).GetPFP(), subtitle: new(en: user.GetUserByID(fromID).GetUsername() + " has sent you a message!"), headings: new(en: user.GetUserByID(fromID).GetUsername() + " has sent you a message!"), contents: new(en: content), appId: "697dd96c-162f-4af5-82a4-e42990cdec84"); // Notification | 

            try
			{
				// Create notification
				CreateNotificationSuccessResponse result = apiInstance.CreateNotification(notification);
				Console.WriteLine(result);
			}
			catch (ApiException  e)
			{
				Console.WriteLine("Exception when calling DefaultApi.CreateNotification: " + e.Message );
				Console.WriteLine("Status Code: "+ e.ErrorCode);
				Console.WriteLine(e.StackTrace);
			}
		}
	}
}