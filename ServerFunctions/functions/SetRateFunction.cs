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
	class SetRateFunction : ServerFunction
	{
		public static SetRateFunction srf = new();
		public SetRateFunction()
		{
			this.activation = "setRate";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{
			try
			{
				sRequest = sRequest.Replace("setRate", "");
				string[] args = sRequest.Split(",");
				user accref = user.GetUserByID(int.Parse(args[0]));
				accref.SetRate(float.Parse(args[1]));
				return "Successfully set new rate";
			}
			catch(Exception ex)
			{
				Console.WriteLine("error occured, error details : " + ex);
				return "could not set new rate";
			}
		}
		
	}
}