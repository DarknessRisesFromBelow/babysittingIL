namespace YMA.ServerHandling
{
	using System;  
	using System.IO;  
	using System.Net;  
	using System.Net.Sockets;  
	using System.Text;  
	using System.Threading;
	using YMA.Management;

	public class Handler
	{
		// enums and variables

				int Port;
				TcpListener myListener;
				public Handler(int port)
				{
					Port = port;
					myListener = new TcpListener(Port);  
				}
		
		// server starting

				public void StartServer()
				{
					try
					{
						myListener.Start();  
						Console.WriteLine("Web Server Running... Press ^C to Stop...");  
						Thread th = new Thread(new ThreadStart(StartListen));  
						th.Start();  
					}
					catch (Exception e)
					{
						Console.WriteLine("An Exception Occurred while Listening :" + e.ToString());  
					}
				}

		// writing to browser

				public void SendToBrowser(Byte[] bSendData, ref Socket mySocket)  
				{  
					int numBytes = 0;  
					try  
					{  
						if (mySocket.Connected)  
						{  
							if ((numBytes = mySocket.Send(bSendData, bSendData.Length, 0)) == -1)  
							Console.WriteLine("Socket Error cannot Send Packet");  
							else  
							{  
								Console.WriteLine("No. of bytes send {0}", numBytes);  
							}  
						}  
						else Console.WriteLine("Connection Dropped....");  
					}  
					catch (Exception e)  
					{  
						Console.WriteLine("Error Occurred : {0} ", e);  
					}  
				}  
		
				public void SendToBrowser(String sData, ref Socket mySocket)  
				{  
					SendToBrowser(Encoding.ASCII.GetBytes(sData), ref mySocket);  
				}
		
				public void SendHeader(string sHttpVersion, string sMIMEHeader, int iTotBytes, string sStatusCode, ref Socket mySocket)  
				{  
					String sBuffer = "";  
					// if Mime type is not provided set default to text/html  
					if (sMIMEHeader.Length == 0)  
					{  
						sMIMEHeader = "text/html";// Default Mime Type is text/html  
					}  
					sBuffer +=  sHttpVersion + sStatusCode + "\r\n";  
					sBuffer += "Server: cx1193719-b\r\n";  
					sBuffer += "Content-Type: " + sMIMEHeader + "\r\n";  
					sBuffer += "Accept-Ranges: bytes\r\n";  
					sBuffer += "Content-Length: " + iTotBytes + "\r\n\r\n";  
					Byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);  
					SendToBrowser(bSendData, ref mySocket);  
					//Console.WriteLine("Total Bytes : " + iTotBytes.ToString());  
				}  

		// write messages to browser (based on presets)

				public void SuccessMessage(string Message, Socket mySocket, string sHttpVersion)
				{
					string message = "Successfully " + Message;
					SendHeader(sHttpVersion, "", message.Length, "200 OK", ref mySocket);
					SendToBrowser(message , ref mySocket);
				}	

				public void error (int code, Socket mySocket, string sHttpVersion)
				{
					string message = "Error! <br>Code :" + code;
					SendHeader(sHttpVersion, "", message.Length, "400 Bad Request", ref mySocket);
					SendToBrowser(message , ref mySocket);
				}
		
				public void error (string Message, Socket mySocket, string sHttpVersion)
				{
					string message = "Error! <br>" + Message;
					SendHeader(sHttpVersion, "", message.Length, "400 Bad Request", ref mySocket);
					SendToBrowser(message , ref mySocket);
				}

		// request accepting and handling

				public void StartListen() 
				{  
					int iStartPos = 0;  
					String sRequest;  
					while (true)  
					{  

						// Listening and server shit

							//Accept a new connection  
							Socket mySocket = myListener.AcceptSocket();  
							Console.WriteLine("Socket Type " + mySocket.SocketType);  
							if (mySocket.Connected)  
							{	  
									//Console.WriteLine("\nClient Connected!!\n==================\n CLient IP { 0}\n", IPAddress.Parse(((IPEndPoint)mySocket.RemoteEndPoint).Address.ToString()));  
									//make a byte array and receive data from the client   
								Byte[] bReceive = new Byte[1024];  
								int i = mySocket.Receive(bReceive, bReceive.Length, 0);  
									//Convert Byte to String  
								string sBuffer = Encoding.ASCII.GetString(bReceive);  
									//At present we will only deal with GET type  
								if (sBuffer.Substring(0, 3) != "GET")  
								{  
									Console.WriteLine("Only Get Method is supported..");  
									mySocket.Close();  
									return;  
								}  
									// Look for HTTP request  
								iStartPos = sBuffer.IndexOf("HTTP", 1);  
									// Get the HTTP text and version e.g. it will return "HTTP/1.1"  
								string sHttpVersion = sBuffer.Substring(iStartPos, 8);  
									// Extract the Requested Type and Requested file/directory  
								sRequest = sBuffer.Substring(0, iStartPos - 1);  
									//Replace backslash with Forward Slash, if Any  
								sRequest = sRequest.Replace("\\", "/").Replace("%22","'").Replace("%20", " ").Replace("GET /", "");
							
						// request handling
		
							if(sRequest.Contains("CreateUser"))
							{
								try
								{
									RegistrationManager.CreateAccount(sRequest.Replace("CreateUser", "").Replace("'",""));
									SuccessMessage("Created User", mySocket, sHttpVersion);
								}
								catch(Exception e)
								{
									Console.WriteLine("error "+ e + " Occurred while creating user");
									error(54, mySocket, sHttpVersion);
								}
							}
		
		
							else if(sRequest.Contains("RemoveUser"))
							{
								try
								{
									RemovalManager.RemoveAccount(int.Parse(sRequest.Replace("RemoveUser", "")));
									SuccessMessage("Removed User", mySocket, sHttpVersion);
								}
								catch(Exception e)
								{
									Console.WriteLine("error "+ e + " Occurred while removing user");									
									error(55, mySocket, sHttpVersion);
								}
							}
		
		
							else if(sRequest.Contains("MailUser"))
							{
								try
								{
									GeneralManagement.activeAccounts[(int.Parse(sRequest.Replace("MailUser", "")) - 1)].SendMail("hello there! welcome to YMA", "<H1> Hello, World! </H1>");
									SuccessMessage("Mailed User", mySocket, sHttpVersion);
								}
								catch(Exception e)
								{
									Console.WriteLine("error "+ e + " Occurred while mailing user");
									error(56, mySocket, sHttpVersion);
								}
							}
		
		
							else if(sRequest.Contains("GetUserData"))
							{
								try
								{
									Account accref = GeneralManagement.activeAccounts[int.Parse(sRequest.Replace("GetUserData", "")) - 1];
									SuccessMessage("Got User Info. <br>" + string.Format("{0},{1},{2}", accref.GetUsername(), accref.GetPassword(), accref.GetEmailAddress()), mySocket, sHttpVersion);
								}
								catch(Exception e)
								{
									Console.WriteLine("error "+ e + " Occurred while getting user data");
									error(57, mySocket, sHttpVersion);
								}
							}
							}  
					}  
				}  
	}
}