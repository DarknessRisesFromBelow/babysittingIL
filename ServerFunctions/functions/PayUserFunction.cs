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
	class PayUserFunction : ServerFunction
	{
		public static PayUserFunction puf = new();
		public PayUserFunction()
		{
			this.activation = "PayUser";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{
			try
			{	
				string[] args = sRequest.Replace("PayUser", "").Split(",");
				if(sessionManager.validate(client.Client.RemoteEndPoint,args[args.Length - 1],int.Parse(args[0])))
				{
					user.GetUserByID(int.Parse(args[0])).transferMoney(int.Parse(args[1]), int.Parse(args[2]));
					return "Successfully passed money around!";
				}
				else
				{
					throw new Exception("could not finish money transfer due to account verification failing.");
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("error occured, error details : " + e);
				return "could not pass money between the accounts.";
			}
		}
	}
}