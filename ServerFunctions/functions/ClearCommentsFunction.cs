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
	class ClearCommentsFunction : ServerFunction
	{
		public static ClearCommentsFunction ccf = new();
		public ClearCommentsFunction()
		{
			this.activation = "ClearComments";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{
			user.GetUserByID(0).clearComments();
			return "done.";
		}
		
	}
}