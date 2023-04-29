using System.Collections.Generic;
using YMA.Management;
using babysittingIL.UserManagement;
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
			Parallel.For(0, messagesArr.Length * messagesArr.Length, i=>
			{
				i = i % (messagesArr.Length - 1);
				if(i > 0 && ulong.Parse(messagesArr[i].Split(":")[4]) > ulong.Parse(messagesArr[i - 1].Split(":")[4]))
				{
					string temp = messagesArr[i-1];
					messagesArr[i-1] = messagesArr[i];
					messagesArr[i] = temp;
				}
			});
			messages = string.Join("||", messagesArr);
			Console.WriteLine("messages were : " + messages);
			return messages;
		}
	}
}