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
	class MessageUserFunction : ServerFunction
	{
		public static MessageUserFunction muf = new();
		public MessageUserFunction()
		{
			this.activation = "MessageUser";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{
			try
			{
				sRequest = sRequest.Replace("MessageUser", "");
				string[] args = sRequest.Split(",");
				if(sessionManager.validate(client.Client.RemoteEndPoint,args[args.Length - 1],int.Parse(args[0])))
				{	
					string message = "";
					for(int o = 2; o < args.Length - 1;o++)
					{
						if(o + 1 != args.Length - 1)
						{
							message += args[o] + ",";
						}
						else
						{
							message += args[o];
						}
					}
					Console.WriteLine(int.Parse(args[0])+","+ int.Parse(args[1])+","+ message);
					new Message(int.Parse(args[0]), int.Parse(args[1]),message);
					return "sent message.";
				}
				else
				throw new Exception("could not verify user, did not allow request to happen.");

			}
			catch (Exception e)
			{
				Console.WriteLine("error occured, error details : " + e);
				return "could not send message because either sender or reciver ID are not valid!";
			}
		}
	}
}