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
				sRequest = sRequest.Replace("GetEvents", "");
				string[] parts = sRequest.Split(",");
				user accref = user.GetUserByID(int.Parse(parts[0]));
				return "" + accref.getEvents();	
			}
			catch(Exception ex)
			{
				Console.WriteLine("error occured, error details : " + ex);
				return "could not get events.";
			}
		}
	}
}