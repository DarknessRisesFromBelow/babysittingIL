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
	class GetFreeClustersFunction : ServerFunction
	{
		public static GetFreeClustersFunction gfcf = new();
		public GetFreeClustersFunction()
		{
			this.activation = "GetFreeClusters";
			ServerFunction.functions.Add(this);
		}
		
		public override string run(string sRequest, TcpClient client)
		{						
			try
			{
				Cluster[] arr = ClusterManager.mgr.getFreeClusters();
				return "free clusters : " + arr.Length;
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
				return "could not get free clusters";
			}
		}
	}
}