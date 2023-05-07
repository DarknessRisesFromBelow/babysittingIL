namespace babysittingIL.calendarManagement
{
	public class calendarManager
	{
		public static void addEvent(int userID, string startDate, float length)
		{
			try
			{
				calendar.addEvent(userID, length, startDate);
			}
			catch
			{
				throw new Exception("could not add Event.");
			}
		}
		
		public string getTakenTimes(int userID)
		{
			try
			{
				return calendar.getEvents(userID);
			}
			catch
			{
				throw new Exception("error at getting events.");
			}
		}

	}
}