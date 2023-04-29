using System.Collections.Generic;
using YMA.Management;
using babysittingIL.UserManagement;
using babysittingIL.sorting;
using System.Linq;

namespace babysittingIL.Messaging
{
	class MessagingManager
	{
		public static List<MessageLists> Lists = new();

		public static string ReadAll(int index)
		{
			string messages = "";
			Console.WriteLine("index : " + index);
			Parallel.For(1, Lists[index].Messages.Count, pos=>
			{
				int i = Lists[index].Messages.Count - pos;
				messages += Lists[index].Messages[i].sender + " : " + Lists[index].Messages[i].message + " : " + user.GetUserByID(Lists[index].Messages[i].sender).GetUsername() + " : " + "0" + " : " + Lists[index].Messages[i].timeStamp + " : " + user.GetUserByID(Lists[index].Messages[i].sender).GetPFP() +" ||";
			});
			Parallel.For(0, Lists.Count, listIndex =>
			{
				for(int i = 0; i < Lists[listIndex].Messages.Count; i++)
				{
					if(Lists[listIndex].Messages[i].sender == index)
					{
						messages += Lists[listIndex].Messages[i].reciver + " : " + Lists[listIndex].Messages[i].message + " : " + user.GetUserByID(Lists[listIndex].Messages[i].reciver).GetUsername() + " : " + "1" + " : " + Lists[listIndex].Messages[i].timeStamp + " : " + user.GetUserByID(Lists[listIndex].Messages[i].sender).GetPFP() + " ||";		
					}
				}
			});
			

			// this code sucks, change it for something better
			string[] messagesArr = messages.Split("||");
			algorithms.gnomeSort(messagesArr,messagesArr.Length - 1);
			messages = string.Join("||", messagesArr);
			Console.WriteLine("messages were : " + messages);
			return messages;
		}
	}
}