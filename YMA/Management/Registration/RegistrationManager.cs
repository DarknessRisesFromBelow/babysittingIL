namespace YMA.Management
{
	class RegistrationManager
	{
		public static Account CreateAccount(string Path)
		{
			string[] strings = Path.Split(",");
			Account NewAcc = new Account(strings[0],strings[1],strings[2]);
			return NewAcc;
		}

		public static Account CreateAccount(string Username, string password, string Email)
		{
			Account NewAcc = new Account(Username, password, Email);
			return NewAcc;
		}
	}
}