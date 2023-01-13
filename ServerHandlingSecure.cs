namespace babysittingIL.ServerHandling
{
	using System;  
	using System.IO;  
	using System.Net;  
	using System.Net.Sockets;
	using System.Net.Security;
	using System.Security.Authentication;
	using System.Security.Cryptography.X509Certificates;  
	using System.Text;  
	using System.Threading;
	using babysittingIL.UserManagement;
	using babysittingIL.UserExperience;
	using babysittingIL.Messaging;
	using babysittingIL.UserManagement.location;
	using babysittingIL.sessionManagement;


	public class SecureHandler
	{
		static X509Certificate serverCertificate = null;
		public static HomeManager manager;
		// The certificate parameter specifies the name of the file
		// containing the machine certificate.
		public static void StartServer(int port, string certificate,string password)
		{
			serverCertificate = new X509Certificate(certificate, password);
			TcpListener listener = new TcpListener(IPAddress.Any, port);
			manager = new HomeManager();
			listener.Start();
			while (true)
			{
				Console.WriteLine("Waiting for a client to connect...");
				TcpClient client = listener.AcceptTcpClient();
				ProcessClient(client);
			}
		}

		static void sendData(string data, ref SslStream stream)
		{
			String sBuffer = "";  
			int iTotBytes = Encoding.UTF8.GetBytes(data).Length;  
			sBuffer +=  "HTTP/1.1 200\r\n";  
			sBuffer += "Server: cx1193719-b\r\n";
			sBuffer += "Access-Control-Allow-Origin : *\r\n"; 
			sBuffer += "Content-Type: text/html \r\n";  
			sBuffer += "Accept-Ranges: bytes\r\n";  
			sBuffer += "Content-Length: " + iTotBytes + "\r\n\r\n";  
			Byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);  
			stream.Write(bSendData);
			Byte[] secondData = Encoding.UTF8.GetBytes(data);
			stream.Write(secondData);
		}

		static void ProcessClient (TcpClient client)
		{
			SslStream sslStream = new SslStream(client.GetStream(), false);
			try
			{
				
				sslStream.AuthenticateAsServer(serverCertificate, clientCertificateRequired: false, checkCertificateRevocation: false);
				
				int iStartPos = 0;
				StringBuilder messageData = new StringBuilder();
				Byte[] bReceive = new Byte[1024];  
				int i = sslStream.Read(bReceive, 0, bReceive.Length);  
				//Convert Byte to String
				Decoder decoder = Encoding.UTF8.GetDecoder();  
				char[] chars = new char[decoder.GetCharCount(bReceive,0,i)];
				decoder.GetChars(bReceive, 0, i, chars,0);
				messageData.Append(chars);
				string sBuffer = messageData.ToString();  
				// Look for HTTP request  
				iStartPos = sBuffer.IndexOf("HTTP", 1);  
				// Get the HTTP text and version e.g. it will return "HTTP/1.1"  
				string sHttpVersion = sBuffer.Substring(iStartPos, 8);  
				// Extract the Requested Type and Requested file/directory  
				string sRequest = sBuffer.Substring(0, iStartPos - 1);  
				//Replace backslash with Forward Slash, if Any  
				sRequest = sRequest.Replace("\\", "/").Replace("%22","'").Replace("%20", " ").Replace("GET /", "").Replace("&",",");
				




				if(sRequest.Contains("CreateUser"))
				{
					try
					{
						if(user.MakeNew(sRequest.Replace("CreateUser", "").Replace("'","")) == -1)
							throw new InvalidOperationException("could not create the user!");
						else
							sendData("Created User ", ref sslStream);
					}
					catch(Exception e)
					{
						sendData("54", ref sslStream);
					}
				}

				else if(sRequest.Contains("GetUserData"))
				{
					try
					{
						user accref = user.GetUserByID(int.Parse(sRequest.Replace("GetUserData", "")));
						sendData("Got User Info. <br>" + accref.GetData(), ref sslStream);
					}
					catch(Exception e)
					{
						sendData("57", ref sslStream);
					}
				}
				else if(sRequest.Contains("AddRating"))
				{
					try
					{
						user accref = user.GetUserByID(int.Parse(sRequest.Replace("AddRating", "").Split(",")[2]));
						accref.AddScore(int.Parse(sRequest.Replace("AddRating", "").Split(",")[0]),int.Parse(sRequest.Replace("AddRating", "").Split(",")[1]));
					}
					catch(Exception ex)
					{

					}
				}	
				else if (sRequest.Contains("MessageUser"))
				{
					try
					{
						sRequest = sRequest.Replace("MessageUser", "");
						string[] args = sRequest.Split(",");
						Console.WriteLine(int.Parse(args[0])+","+ int.Parse(args[1])+","+ args[2]);
						if(sessionManager.validate(client.Client.RemoteEndPoint,args[3],int.Parse(args[0])))
						{
							new Message(int.Parse(args[0]), int.Parse(args[1]), args[2]);
							sendData("sent message.", ref sslStream);
						}
						else
							throw new Exception("could not verify user, did not allow request to happen.");

					}
					catch (Exception e)
					{
						sendData("could not send message because either sender or reciver ID are not valid!", ref sslStream);
					}
				}

				else if (sRequest.Contains("GetAllMessages"))
				{
					try
					{
						sRequest = sRequest.Replace("GetAllMessages","");
						string[] requestData = sRequest.Split(",");
						if(sessionManager.validate(client.Client.RemoteEndPoint,requestData[1],int.Parse(requestData[0])))
							sendData("Messages : " + MessagingManager.ReadAll(int.Parse(requestData[0])), ref sslStream);
						else
							throw new Exception("could not verify user, did not allow request to happen.");
					}
					catch(Exception ex)
					{
						Console.WriteLine(ex);
						sendData("invalid User!", ref sslStream);
					}
				}
				else if(sRequest.Contains("GetUserHome"))
				{
					try
					{
						sRequest = sRequest.Replace("GetUserHome", "");
						string[] requestData = sRequest.Split(",");
						if(sessionManager.validate(client.Client.RemoteEndPoint,requestData[1],int.Parse(requestData[0])))
							sendData(manager.GetUserHome(int.Parse(requestData[0])), ref sslStream);
						else
							throw new Exception("could not verify user, did not allow request to happen.");
					}
					catch (Exception ex)
					{
						sendData("invalid user!",ref sslStream);
						Console.WriteLine(ex);
					}
				}
				else if (sRequest.Contains("setGeolocation"))
				{
					try 
					{
						sRequest = sRequest.Replace("setGeolocation","");
						string[] requestData = sRequest.Split(",");
						if(sessionManager.validate(client.Client.RemoteEndPoint,requestData[3],int.Parse(requestData[0])))
						{
							user.GetUserByID(int.Parse(requestData[0])).setLocation((double.Parse(requestData[1]),double.Parse(requestData[2])));
							Console.WriteLine("user " + int.Parse(requestData[0]) + "'s location has been set as " +user.GetUserByID(int.Parse(requestData[0])).getLocation());
							sendData("set user's location to " + user.GetUserByID(int.Parse(requestData[0])).getLocation(), ref sslStream);
						}
						else
							throw new Exception("could not verify user, did not allow request to happen.");
					}
					catch(Exception ex)
					{
						sendData("could not set user location.",ref sslStream);
					}
				}
				else if(sRequest.Contains("login"))
				{
					try
					{
						sRequest = sRequest.Replace("login", "");
						string[] requestData = sRequest.Split(",");
						Console.WriteLine("attempting to sign in user " + requestData[0]+" with password " + requestData[1]);
						int id = user.GetIdByName(requestData[0]);
						Console.WriteLine("id : " + id);
						user myUser = user.GetUserByID(id);
						Console.WriteLine(myUser.GetUsername()+"'s password is " + myUser.GetPassword());
						myUser.login(requestData[1], client.Client.RemoteEndPoint);
						sendData("logged in, needed info is " + myUser.getSessionID() + "," + myUser.GetID(), ref sslStream);
					}
					catch(Exception ex)
					{
						sendData("log in" + ex, ref sslStream);
					}
				}
			}
			catch (AuthenticationException e)
			{
				Console.WriteLine("Exception: {0}", e.Message);
				if (e.InnerException != null)
				{
					Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
				}
				Console.WriteLine ("Authentication failed - closing the connection.");
				sslStream.Close();
				client.Close();
				return;
			}
			finally
			{
				sslStream.Close();
				client.Close();
			}
		}
	}
}




//	Command Table : 
//	
//	CreateUser : takes an int and 3 strings. (1: UserType, 2: username, 3: email, 4: password)	
//	MessageUser : takes 2 ints and a string.  (1: from, 2: to, 3: message)
//	GetAllMessages : takes a single int. (1: id of user you want to get messages of)
//  GetUserHome : takes a single int (1: ID of user who gets his home page)