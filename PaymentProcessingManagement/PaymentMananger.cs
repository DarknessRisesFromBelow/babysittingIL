using System.Net.Http;
using System;
using babysittingIL.Constants;
using babysittingIL.util.webHelper;

namespace babysittingIL.Payments
{
	class PaymentsManager
	{
		public static void payUser(int userID, float amount)
		{
			webFunctions helper = new webFunctions();
			try
			{
				string request = string.Format("http://localhost:5678/payUser{0},{1}", userID, amount);
				helper.HttpGet(request);
			}
			catch(Exception ex)
			{
				Console.WriteLine("problem occured while making a payment. ");
			}
		}
	}
}