using babysittingIL.ServerHandling;
using babysittingIL.Constants;

namespace babysittingIL
{
	class Entry
	{
		public static void Main(string[] args)
		{
			string certificate = "./certs/test/5/certificate.pfx";
			//SecureHandler.StartServer(443,certificate,Consts.PfxPassword);	
			new Handler(80).StartServer();		
		}
	}
}