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
using YMA.Authorization;
using babysittingIL.storingManagement;
using babysittingIL.UserManagement;
using babysittingIL.UserExperience;
using babysittingIL.Messaging;
using babysittingIL.UserManagement.location;
using babysittingIL.sessionManagement;
namespace babysittingIL.ServerFunctions
{
	class AddUserToClusterFunction : ServerFunction
	{
		public static AddUserToClusterFunction autf = new();
		public AddUserToClusterFunction()
		{
			this.activation = "AddUserToCluster";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{						
			try
			{
				new AuthAcc("TEST:USER0", "THISDOESNOTMATTERITWOULDNOTGETUSEDANYWAY", "YMACORPEMAIL@GMAIL.COM");
				return "Succesfully added user";
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
				return "could not add user";
			}
		}
	}
}