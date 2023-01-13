using babysittingIL.UserManagement;
using System.Net;  
namespace babysittingIL.sessionManagement
{
	public class session
	{
		string id = "a";
		user linkedUser;
		EndPoint device;
		public session(user UserLoggedIn,EndPoint Device)
		{
			linkedUser = UserLoggedIn;
			device = Device;
			sessionManager.ActiveSessions.Add(this);
			id += sessionManager.ActiveSessions.IndexOf(this); 
		}
		public void close() => sessionManager.ActiveSessions.RemoveAt(sessionManager.ActiveSessions.IndexOf(this));
		public string getSessionID() => id;
		public user getLinkedUser() => linkedUser;
		public EndPoint getDevice() => device;
	}
	public class sessionManager
	{
		public static List<session> ActiveSessions = new();
		public static session getSessionByID(string ID)
		{
			ID = ID.Replace("a","");
			return ActiveSessions[int.Parse(ID)];
		}

		public static bool validate(EndPoint connectedDevice, string sessionID, int UserID)
		{
			Console.WriteLine(ActiveSessions.Count);
			Console.WriteLine("starting validation...");
			Console.WriteLine("EndPoint : " + connectedDevice.ToString());
			Console.WriteLine("Saved end point : " + getSessionByID(sessionID).getDevice());
			return getSessionByID(sessionID).getLinkedUser().Equals(user.GetUserByID(UserID)) && getSessionByID(sessionID).getDevice().ToString().Remove(getSessionByID(sessionID).getDevice().ToString().IndexOf(":")).Equals(connectedDevice.ToString().Remove(connectedDevice.ToString().IndexOf(":")));
		}

	}
}