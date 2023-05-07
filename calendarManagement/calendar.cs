using System;

namespace babysittingIL.calendarManagement
{
	class calendar
	{
		static List<List<string>> events = new List<List<string>>();
		
		public static void addEvent(int id, float length, string startingTime)
		{
			events[id].Add("" + startingTime + "--" + length);
			removeOldEvents(id);
		}

		public static string getEvents(int id)
		{
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
				if(DateTime.Parse(events[id][i].Split("--")[0]) < DateTime.Today)
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