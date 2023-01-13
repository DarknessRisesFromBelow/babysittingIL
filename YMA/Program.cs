using YMA.Management;
using YMA.ServerHandling;

namespace YMA.Debugging
{
	public class Debugger
	{
		public static void Test()
		{
			Console.WriteLine(RegistrationManager.CreateAccount("TEST,TEST,TEST@TEST.com").GetEmailAddress());
			RemovalManager.RemoveAccount(1);
			Handler handler = new Handler(8080);
			handler.StartServer();
		}
	}
}


