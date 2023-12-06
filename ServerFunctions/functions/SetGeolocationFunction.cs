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
	class SetGeolocationFunction : ServerFunction
	{
		public static SetGeolocationFunction puf = new();
		public SetGeolocationFunction()
		{
			this.activation = "setGeolocation";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{
			try 
			{
				sRequest = sRequest.Replace("setGeolocation","");
				string[] requestData = sRequest.Split(",");
				if(sessionManager.validate(client.Client.RemoteEndPoint,requestData[3],int.Parse(requestData[0])))
				{
					user.GetUserByID(int.Parse(requestData[0])).setLocation((double.Parse(requestData[1]),double.Parse(requestData[2])));
					Console.WriteLine("user " + int.Parse(requestData[0]) + "'s location has been set as " + user.GetUserByID(int.Parse(requestData[0])).getLocation());
					return "set user's location to " + user.GetUserByID(int.Parse(requestData[0])).getLocation();
				}
				else
				{
					throw new Exception("could not verify user, did not allow request to happen.");
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine("error occured, error details : " + ex);
				return "could not set user location.";
			}
		}
	}
}