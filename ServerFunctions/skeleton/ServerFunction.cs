using System;  
using System.IO;  
using System.Net;
using System.Web;
using System.Net.Sockets;	
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;  
using System.Text;  
using System.Threading;
using babysittingIL.storingManagement;
using babysittingIL.UserManagement;
using babysittingIL.UserExperience;
using babysittingIL.Messaging;
using babysittingIL.UserManagement.location;
using babysittingIL.sessionManagement;

namespace babysittingIL.ServerFunctions
{
	abstract class ServerFunction
	{

		public static List<ServerFunction> functions = new();
		public string activation;
		public abstract string run(string sRequest, TcpClient client);
		public virtual string getActivation() => activation;
	}
}