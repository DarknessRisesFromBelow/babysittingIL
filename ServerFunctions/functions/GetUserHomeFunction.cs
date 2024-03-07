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
using babysittingIL.ServerHandling;

namespace babysittingIL.ServerFunctions
{
	class GetUserHomeFunction : ServerFunction
	{
		public static GetUserHomeFunction puf = new();
		public GetUserHomeFunction()
		{
			this.activation = "GetUserHome";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{
			try
			{
				sRequest = sRequest.Replace("GetUserHome", "");
				string[] requestData = sRequest.Split(",");
				if(sessionManager.validate(client.Client.RemoteEndPoint,requestData[1],int.Parse(requestData[0])))
				{
					return SecureHandler.manager.GetUserHome(int.Parse(requestData[0]));
				}
				else
				{
					throw new Exception("could not verify user, did not allow request to happen.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return "invalid user!";
			}
		}
	}
}