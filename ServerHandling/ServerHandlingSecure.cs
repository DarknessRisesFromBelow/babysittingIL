namespace babysittingIL.ServerHandling
{
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

		//public SecureHandler()
		//{
		//	manager = new HomeManager();
		//}


		/// <summary>
		/// Starts a server.
		/// </summary>
		///
		/// <param name="port">The port</param>
		/// <param name="certificate">The certificate</param>
		/// <param name="password">The password</param>
		public static void StartServer(int port, string certificate,string password)
		{
			manager = new HomeManager();
			serverCertificate = new X509Certificate(certificate, password);
			TcpListener listener = new TcpListener(IPAddress.Any, port);
			listener.Start();
			while (true)
			{
				Console.WriteLine("Waiting for a client to connect...");
				TcpClient client = listener.AcceptTcpClient();
				ProcessClient(client);
			}
		}

		/// <summary>
		/// Sends data back to the client.
		/// </summary>
		///
		/// <param name="data">The data</param>
		/// <param name="stream">The stream</param>
		static void sendData(string data, ref SslStream stream)
		{
			String sBuffer = "";  
			int iTotBytes = Encoding.UTF8.GetBytes(data).Length;  
			sBuffer +=  "HTTP/1.1 200\r\n";  
			sBuffer += "Server: BBIL-SERVER-0\r\n";
			sBuffer += "Access-Control-Allow-Origin : *\r\n"; 
			sBuffer += "Content-Type: text/html \r\n";  
			sBuffer += "Accept-Ranges: bytes\r\n";  
			sBuffer += "Content-Length: " + iTotBytes + "\r\n\r\n";  
			Byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);  
			stream.Write(bSendData);
			Byte[] secondData = Encoding.UTF8.GetBytes(data);
			stream.Write(secondData);
		}

		/// <summary>
		/// function processes the HTTP/S request.
		/// </summary>
		///
		/// <param name="client">The client</param>
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
				sRequest = HttpUtility.UrlDecode(sRequest);



				try
				{
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
							Console.WriteLine("error occured, error details : " + e);
							sendData("54", ref sslStream);
						}
					}

					else if(sRequest.Contains("PayUser"))
					{
						try
						{	
							string[] args = sRequest.Replace("PayUser", "").Split(",");
							if(sessionManager.validate(client.Client.RemoteEndPoint,args[args.Length - 1],int.Parse(args[0])))
							{
								user.GetUserByID(int.Parse(args[0])).transferMoney(int.Parse(args[1]), int.Parse(args[2]));
								sendData("Successfully passed money around!", ref sslStream);
							}
							else
							{
								throw new Exception("could not finish money transfer due to account verification failing.");
							}
						}
						catch(Exception e)
						{
							Console.WriteLine("error occured, error details : " + e);
							sendData("could not pass money between the accounts.", ref sslStream);
						}
					}
					else if(sRequest.Contains("ClearComments"))
					{
						user.GetUserByID(0).clearComments();
						sendData("done.", ref sslStream);
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
							Console.WriteLine("error occured, error details : " + e);
							sendData("57", ref sslStream);
						}
					}
	/////////////////////////////	obselete, use AddReview instead.	///////////////////////////// 					
					else if(sRequest.Contains("AddRating"))
					{
						try
						{
							user accref = user.GetUserByID(int.Parse(sRequest.Replace("AddRating", "").Split(",")[2]));
							accref.AddScore(int.Parse(sRequest.Replace("AddRating", "").Split(",")[0]),int.Parse(sRequest.Replace("AddRating", "").Split(",")[1]));
							sendData("added rating!", ref sslStream);	
						}
						catch(Exception ex)
						{
							Console.WriteLine("error occured, error details : " + ex);
							sendData("could not add rating!", ref sslStream);	
						}
					}
	//////////////////////////////////////////////////////////////////////////////////////////////////
					else if (sRequest.Contains("setPfp"))
					{
						try
						{
							sRequest = sRequest.Replace("setPfp", "");
							string[] args = sRequest.Split(",");
							user accref = user.GetUserByID(int.Parse(args[0]));
							accref.SetPFP(args[1]);
							sendData("Successfully set new pfp", ref sslStream);
						}
						catch(Exception ex)
						{
							Console.WriteLine("error occured, error details : " + ex);
							sendData("could not set new pfp", ref sslStream);
						}
					}
					else if (sRequest.Contains("setRate"))
					{
						try
						{
							sRequest = sRequest.Replace("setRate", "");
							string[] args = sRequest.Split(",");
							user accref = user.GetUserByID(int.Parse(args[0]));
							accref.SetRate(float.Parse(args[1]));
							sendData("Successfully set new rate", ref sslStream);
						}
						catch(Exception ex)
						{
							Console.WriteLine("error occured, error details : " + ex);
							sendData("could not set new rate", ref sslStream);
						}
					}	
					else if (sRequest.Contains("setBio"))
					{
						try
						{
							string newBio = "";
							sRequest = sRequest.Replace("setBio", "");
							string[] args = sRequest.Split(",");
							user accref = user.GetUserByID(int.Parse(args[0]));

							newBio += args[1];
							for(int q = 2; q < args.Length; q++)
							{
								if(args.Length != 2)
								{
									newBio += "," + args[q];
								}
							}
							accref.SetBio(newBio);
							sendData("Successfully set new bio", ref sslStream);
						}
						catch(Exception ex)
						{
							Console.WriteLine("error occured, error details : " + ex);
							sendData("Could not set new bio", ref sslStream);
						}
					}
					else if (sRequest.Contains("getComments"))
					{
						try
						{
							sRequest = sRequest.Replace("getComments", "");
							user accref = user.GetUserByID(int.Parse(sRequest));
							sendData("---REVIEWS-START---\n" + accref.GetReviews() + "---REVIEWS-END---", ref sslStream);
						}
						catch(Exception ex)
						{
							Console.WriteLine(ex);
							sendData("REVIEWGETTINGERROR 51", ref sslStream);
						}
					}
					else if (sRequest.Contains("MessageUser"))
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
								sendData("sent message.", ref sslStream);
							}
							else
								throw new Exception("could not verify user, did not allow request to happen.");

						}
						catch (Exception e)
						{
							Console.WriteLine("error occured, error details : " + e);
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
					else if (sRequest.Contains("AddReview"))
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
							sendData("Succesfully added review",ref sslStream);
						}
						catch(Exception ex)
						{
							Console.WriteLine(ex);
							sendData("could not add review",ref sslStream);

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
							Console.WriteLine("error occured, error details : " + ex);
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
							sendData("logged in, needed info is " + myUser.getSessionID() + "," + myUser.GetID() + "," + myUser.GetType(), ref sslStream);
						}
						catch(Exception ex)
						{
							Console.WriteLine("error occured, error details : " + ex);
							sendData("log in" + ex, ref sslStream);
						}
					}
				}
				catch(Exception e)
				{
					try
					{
						Console.WriteLine("error encountered.\nerror:\t{0}\ncontinuing...", e.InnerException.Message);
					}
					catch(Exception ex)
					{
						Console.WriteLine("error printing the error message, oops ig... detail of the error: "+ ex);
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