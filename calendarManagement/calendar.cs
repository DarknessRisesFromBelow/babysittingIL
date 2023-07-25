using System;
using babysittingIL.UserManagement;
using babysittingIL.PushMessaging;


namespace babysittingIL.calendarManagement
{
	class calendar
	{
		static List<List<string>> events = new List<List<string>>();
		
		public static void addEvent(int id, float length, string startingTime, int reserverID)
		{
			user accref = user.GetUserByID(reserverID);
			events[id].Add("" + startingTime + "--" + length + "--" + accref.GetPFP() + "--" + accref.GetUsername() + "--" + reserverID);
			Console.WriteLine("Added Event : " + events[id][events[id].Count - 1]);
			PushMessagingManager.sendMessage(reserverID, id, "click here to view the event!", notificationType.reservation);
			removeOldEvents(id);
			Console.WriteLine(getEvents(id));
		}

		public static string getEvents(int id)
		{
			removeOldEvents(id);
			string userEvents = "";
			for(int i = 0; i < events[id].Count; i++)
			{
				if(i != events[id].Count - 1)
					userEvents += events[id][i] + "||";
				else
					userEvents += events[id][i];
			}
			return userEvents;
		}

		public static void removeOldEvents(int id)
		{
			for(int i = 0; i < events[id].Count; i++)
			{
				if(DateTime.Parse(events[id][i].Split("--")[0].Replace(" ", "+")) < DateTime.Today)
				{
					events[id].RemoveAt(i);
				}
			}
		} 

		public static void addUser()
		{
			events.Add(new List<string>());
		}
	}
}