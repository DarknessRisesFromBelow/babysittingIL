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
	class GetEventsFunction : ServerFunction
	{
		public static CreateUserFunction gef = new();
		public GetEventsFunction()
		{
			this.activation = "GetEvents";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{
			try
			{
				if(user.MakeNew(sRequest.Replace("CreateUser", "").Replace("'","")) == -1)
					throw new InvalidOperationException("could not create the user!");
				else
					return "Created User ";
			}
			catch(Exception e)
			{
				Console.WriteLine("error occured, error details : " + e);
				return "54";
			}
		}
	}
}