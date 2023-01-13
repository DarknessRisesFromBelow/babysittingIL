using System.Collections.Generic;
using YMA.Management;
namespace babysittingIL.Messaging
{
	class MessagingManager
	{
		public static List<MessageLists> Lists = new();

		public static string ReadAll(int index)
		{
			string messages = "";
			for(int i  = Lists[index].Messages.Count - 1; i > 0; i--)
			{
				messages += Lists[index].Messages[i].sender + " : " + Lists[index].Messages[i].message + " ||";
			}
			return messages;
		}
	}
}