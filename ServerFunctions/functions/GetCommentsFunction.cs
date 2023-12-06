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
	class GetCommentsFunction : ServerFunction
	{
		public static GetCommentsFunction gcf = new();
		public GetCommentsFunction()
		{
			this.activation = "getComments";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{
			try
			{
				sRequest = sRequest.Replace("getComments", "");
				user accref = user.GetUserByID(int.Parse(sRequest));
				return "---REVIEWS-START---\n" + accref.GetReviews() + "---REVIEWS-END---";
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
				return "REVIEWGETTINGERROR 51";
			}
		}
	}
}