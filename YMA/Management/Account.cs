using System;
using System.Net.Mail;
using System.Net;
using YMA.General;

namespace YMA.Management
{
	public class Account
	{
		
		string username, password;
		int ID = -1;
		Email email;
		public string pfpURL = Constants.ServerURL + "DPFP";

		public Account(string Username, string Password, string Email)
		{
			for(int i = 0; i < GeneralManagement.activeAccounts.Count; i++)
			{
				if(GeneralManagement.activeAccounts[i].GetUsername() == Username || GeneralManagement.activeAccounts[i].GetEmailAddress() == Email)
				{
					//GeneralManagement.activeAccounts.RemoveAt(i);
					// ^ stupid, do not do this.
					Console.WriteLine("there exists a different user with this username/email");
					throw new InvalidOperationException("can not create account with these details, either username or account already exists in YMA");
				}
			}
			username = Username;
			password = Password;
			Console.WriteLine(Email);
			email = new Email(Email);
			GeneralManagement.activeAccounts.Add(this);
			ID = Array.IndexOf(GeneralManagement.activeAccounts.ToArray(), this);
			Console.WriteLine(GetID());
		}
		
		public int GetID() => ID;
		public string GetUsername() => username;
		public string GetPassword() => password;
		public string GetEmailAddress() => email.GetAddress();
		public void SendMail (string subject, string HTML) => email.Send(subject, HTML);
	}

	public struct Email
	{
		string address;
		public Email(string Address)
		{
			address = Address.Replace(" ", "");
		}

		public string GetAddress() => address;

		public void Send(string subject, string HTML)
		{
			SmtpClient client = new(Constants.smtpServer)
			{
				Port = 587,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(Constants.MyMail, Constants.smtpAppID),
				EnableSsl = true,
			};
			MailMessage message = new MailMessage
			{
				From = new MailAddress(Constants.MyMail, "YMA"),
				Subject = Constants.MailSubjectPreset + subject,
				Body = HTML,
				IsBodyHtml = true,
			};
			message.To.Add(address);
			client.Send(message);
		}
	}
}