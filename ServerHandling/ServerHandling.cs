namespace babysittingIL.ServerHandling
{
	using System;  
	using System.IO;  
	using System.Net;  
	using System.Net.Sockets;
	using System.Net.Security;
	using System.Security.Authentication;  
	using System.Text;  
	using System.Threading;
	using babysittingIL.UserManagement;
	using babysittingIL.UserExperience;
	using babysittingIL.Messaging;
	using babysittingIL.UserManagement.location;
	using babysittingIL.sessionManagement;


	public class Handler
	{
		// enums and variables
				HomeManager manager;
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
						manager = new HomeManager();
						//user.MakeNew("1,yair,medinayair1602@gmail.com,helloWorld!");
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
							numBytes = mySocket.Send(bSendData, bSendData.Length, 0);
							if (numBytes == -1)  
								Console.WriteLine("Socket Error cannot Send Packet");  
							else  
							{  
								Console.WriteLine("No. of bytes send {0}", numBytes);  
							}  
						}  
						else
						 	Console.WriteLine("Connection Dropped....");  
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
					sBuffer += "Access-Control-Allow-Origin : *\r\n"; 
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

				public void message(string Message, Socket mySocket, string sHttpVersion)
				{
					string message = Message;
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
						try
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
									// Look for HTTP request  
								iStartPos = sBuffer.IndexOf("HTTP", 1);  
									// Get the HTTP text and version e.g. it will return "HTTP/1.1"  
								string sHttpVersion = sBuffer.Substring(iStartPos, 8);  
									// Extract the Requested Type and Requested file/directory  
								sRequest = sBuffer.Substring(0, iStartPos - 1);  
									//Replace backslash with Forward Slash, if Any  
								sRequest = sRequest.Replace("\\", "/").Replace("%22","'").Replace("%20", " ").Replace("GET /", "").Replace("&",",");
							
						// request handling
		
							if(sRequest.Contains("CreateUser"))
							{
								try
								{
									if(user.MakeNew(sRequest.Replace("CreateUser", "").Replace("'","")) == -1)
										throw new InvalidOperationException("could not create the user!");
									else
										SuccessMessage("Created User ", mySocket, sHttpVersion);
								}
								catch(Exception e)
								{
									error(54, mySocket, sHttpVersion);
								}
							}

							else if(sRequest.Contains("GetUserData"))
							{
								try
								{
									user accref = user.GetUserByID(int.Parse(sRequest.Replace("GetUserData", "")));
									SuccessMessage("Got User Info. <br>" + accref.GetData(), mySocket, sHttpVersion);
								}
								catch(Exception e)
								{
									error(57, mySocket, sHttpVersion);
								}
							}

							else if (sRequest.Contains("MessageUser"))
							{
								try
								{
									sRequest = sRequest.Replace("MessageUser", "");
									string[] args = sRequest.Split(",");
									Console.WriteLine(int.Parse(args[0])+","+ int.Parse(args[1])+","+ args[2]);
									if(sessionManager.validate(mySocket.RemoteEndPoint,args[3],int.Parse(args[0])))
									{
										new Message(int.Parse(args[0]), int.Parse(args[1]), args[2]);
										SuccessMessage("sent message.", mySocket, sHttpVersion);
									}
									else
										throw new Exception("could not verify user, did not allow request to happen.");

								}
								catch (Exception e)
								{
									error("could not send message because either sender or reciver ID are not valid!", mySocket, sHttpVersion);
								}
							}

							else if (sRequest.Contains("GetAllMessages"))
							{
								try
								{
									sRequest = sRequest.Replace("GetAllMessages","");
									string[] requestData = sRequest.Split(",");
									if(sessionManager.validate(mySocket.RemoteEndPoint,requestData[1],int.Parse(requestData[0])))
										SuccessMessage("Messages : " + MessagingManager.ReadAll(int.Parse(requestData[0])), mySocket, sHttpVersion);
									else
										throw new Exception("could not verify user, did not allow request to happen.");
								}
								catch(Exception ex)
								{
									Console.WriteLine(ex);
									error("invalid User!", mySocket, sHttpVersion);
								}
							}
							else if(sRequest.Contains("GetUserHome"))
							{
								try
								{
									sRequest = sRequest.Replace("GetUserHome", "");
									string[] requestData = sRequest.Split(",");
									if(sessionManager.validate(mySocket.RemoteEndPoint,requestData[1],int.Parse(requestData[0])))
										message(manager.GetUserHome(int.Parse(requestData[0])), mySocket,sHttpVersion);
									else
										throw new Exception("could not verify user, did not allow request to happen.");
								}
								catch (Exception ex)
								{
									error("invalid user!",mySocket, sHttpVersion);
									Console.WriteLine(ex);
								}
							}
							else if (sRequest.Contains("setGeolocation"))
							{
								try 
								{
									sRequest = sRequest.Replace("setGeolocation","");
									string[] requestData = sRequest.Split(",");
									if(sessionManager.validate(mySocket.RemoteEndPoint,requestData[3],int.Parse(requestData[0])))
									{
										user.GetUserByID(int.Parse(requestData[0])).setLocation((double.Parse(requestData[1]),double.Parse(requestData[2])));
										Console.WriteLine("user " + int.Parse(requestData[0]) + "'s location has been set as " +user.GetUserByID(int.Parse(requestData[0])).getLocation());
										SuccessMessage("set user's location to " + user.GetUserByID(int.Parse(requestData[0])).getLocation(), mySocket,sHttpVersion);
									}
									else
										throw new Exception("could not verify user, did not allow request to happen.");
								}
								catch(Exception ex)
								{
									error("could not set user location.",mySocket,sHttpVersion);
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
									myUser.login(requestData[1], mySocket.RemoteEndPoint);
									SuccessMessage("logged in, needed info is " + myUser.getSessionID() + "," + myUser.GetID(), mySocket,sHttpVersion);
								}
								catch(Exception ex)
								{
									error("log in" + ex, mySocket,sHttpVersion);
								}
							}
						}

					}
					catch(Exception ex)
					{
						Console.WriteLine("Error Occurred, continuing.");
					}
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