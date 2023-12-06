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
	class AddReviewFunction : ServerFunction
	{
		public static AddReviewFunction puf = new();
		public AddReviewFunction()
		{
			this.activation = "AddReview";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{						
			try
			{
				sRequest = sRequest.Replace("AddReview", "");
				string[] info = sRequest.Split(",");
				user accref = user.GetUserByID(int.Parse(info[0]));
				accref.AddScore(int.Parse(info[info.Length - 1]), int.Parse(info[1]));
				string text = info[2];
				if(info.Length > 4)
				{
					text = "";
					for(int o = 2; o < info.Length - 1; o++)
					{
						text += info[o];
						text += (o != info.Length - 2) ? "," : "";
					}
				}
				accref.AddReview(int.Parse(info[1]), text, int.Parse(info[info.Length - 1]));
				return "Succesfully added review";
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
				return "could not add review";
			}
		}
	}
}