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
	class LoginFunction : ServerFunction
	{
		public static LoginFunction lf = new();
		public LoginFunction()
		{
			this.activation = "login";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{
			try
			{
				sRequest = sRequest.Replace("login", "");
				string[] requestData = sRequest.Split(",");
				Console.WriteLine("attempting to sign in user " + requestData[0]+" with password " + requestData[1]);
				int id = user.GetIdByName(requestData[0]);
				Console.WriteLine("id : " + id);
				user myUser = user.GetUserByID(id);
				Console.WriteLine(myUser.GetUsername()+"'s password is " + myUser.GetPassword());
				myUser.login(requestData[1], client.Client.RemoteEndPoint);
				return "logged in, needed info is " + myUser.getSessionID() + "," + myUser.GetID() + "," + myUser.GetType();
			}
			catch(Exception ex)
			{
				Console.WriteLine("error occured, error details : " + ex);
				return "log in" + ex;
			}
		}
	}
}