using babysittingIL.UserManagement;
namespace babysittingIL.Messaging
{
	struct Message
	{
		public int sender, reciver;
		public string message;
		public Message(int From, int To, string text)
		{
			if(user.GetUserByID(From).Equals(user.NotAUser) || user.GetUserByID(To).Equals(user.NotAUser))
				throw new InvalidOperationException("can not send message, either sender or reciver ID is invalid");
			sender = From;
			reciver = To;
			message = text;
			MessagingManager.Lists[To].Messages.Add(this);
		}
	}
}