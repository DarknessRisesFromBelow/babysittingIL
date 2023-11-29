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
	class ReserveBabysitterFunction : ServerFunction
	{
		public static ReserveBabysitterFunction rbf = new();
		public ReserveBabysitterFunction()
		{
			this.activation = "ReserveBabysitter";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{
			// https://{serverip}/ReserveBabysitter{targetID},{Date},{eventLength},{reserverID},{sessionID}
			try
			{
				String[] parts = sRequest.Replace("ReserveBabysitter", "").Split(",");
				user myUser = user.GetUserByID(int.Parse(parts[0]));
				if(sessionManager.validate(client.Client.RemoteEndPoint,parts[parts.Length - 1],int.Parse(parts[parts.Length - 2])))
				{
					myUser.addEvent(parts[1], float.Parse(parts[2]), int.Parse(parts[parts.Length - 2]));
					return "Successfully added event!";
				}
				else
				{
					throw new Exception("could not verify user identity, did not add event.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("error adding event, error info : " + ex);
				return "could not add event";
			}
		}
	}
}