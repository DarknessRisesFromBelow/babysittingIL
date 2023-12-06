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
	class SetBioFunction : ServerFunction
	{
		public static SetBioFunction sbf = new();
		public SetBioFunction()
		{
			this.activation = "setBio";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{
			try
			{
				string newBio = "";
				sRequest = sRequest.Replace("setBio", "");
				string[] args = sRequest.Split(",");
				user accref = user.GetUserByID(int.Parse(args[0]));

				newBio += args[1];
				for(int q = 2; q < args.Length; q++)
				{
					if(args.Length != 2)
					{
						newBio += "," + args[q];
					}
				}
				accref.SetBio(newBio);
				return "Successfully set new bio";
			}
			catch(Exception ex)
			{
				Console.WriteLine("error occured, error details : " + ex);
				return "Could not set new bio";
			}
		}
		
	}
}