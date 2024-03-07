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
	class SetPfpFunction : ServerFunction
	{
		public static SetPfpFunction spf = new();
		public SetPfpFunction()
		{
			this.activation = "setPfp";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{
			try
			{
				sRequest = sRequest.Replace("setPfp", "");
				string[] args = sRequest.Split(",");
				user accref = user.GetUserByID(int.Parse(args[0]));
				accref.SetPFP(args[1]);
				return "Successfully set new pfp";
			}
			catch(Exception ex)
			{
				Console.WriteLine("error occured, error details : " + ex);
				return "could not set new pfp";
			}
		}
		
	}
}