namespace babysittingIL.calendarManagement
{
	public class calendarManager
	{
		public static void addEvent(int userID, string startDate, float length, int reserverID)
		{
			try
			{
				calendar.addEvent(userID, length, startDate, reserverID);
			}
			catch (Exception ex)
			{
				throw new Exception("could not add Event. Event detail : (" + userID + "," + startDate + "," + length + ")" + "and exception was " + ex);
			}
		}
		
		public static string getTakenTimes(int userID)
		{
			try
			{
				return calendar.getEvents(userID);
			}
			catch (Exception ex)
			{
				throw new Exception("error getting events. exception was " + ex);
			}
		}

	}
}