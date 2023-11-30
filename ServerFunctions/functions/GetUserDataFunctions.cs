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
	class GetUserDataFunction : ServerFunction
	{
		public static GetUserDataFunction gudf = new();
		public ClearCommentsFunction()
		{
			this.activation = "GetUserData";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{
			try
			{
				user accref = user.GetUserByID(int.Parse(sRequest.Replace("GetUserData", "")));
				return "Got User Info. <br>" + accref.GetData();
			}
			catch(Exception e)
			{
				Console.WriteLine("error occured, error details : " + e);
				return "57";
			}
		}
		
	}
}