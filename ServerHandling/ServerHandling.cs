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
	using babysittingIL.ServerFunctions;

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
			manager = new HomeManager();

			// init server functions
			new AddReviewFunction();
			new ClearCommentsFunction();
			new CreateUserFunction();
			new GetAllMessagesFunction();
			new GetCommentsFunction();
			new GetEventsFunction();
			new GetUserDataFunction();
			new GetUserHomeFunction();
			new LoginFunction();
			new MessageUserFunction();
			new PayUserFunction();
			new ReserveBabysitterFunction();
			new SetBioFunction();
			new SetGeolocationFunction();
			new SetPfpFunction();
			new SetRateFunction();
			new AddUserToClusterFunction();
			new GetFreeClustersFunction();
		}
		
		// server starting

		public void StartServer()
		{
			try
			{
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
					{
						Console.WriteLine("Socket Error cannot Send Packet");  
						mySocket.Shutdown(SocketShutdown.Both);
						mySocket.Close();
					}  
					else  
					{  
						Console.WriteLine("No. of bytes send {0}", numBytes);  
						mySocket.Shutdown(SocketShutdown.Both);
						mySocket.Close();
					}  
				}  
				else
					Console.WriteLine("Connection Dropped....");  
			}  
			catch (Exception e)  
			{  
				Console.WriteLine("Error Occurred : {0} ", e);  
				mySocket.Shutdown(SocketShutdown.Both);
				mySocket.Close();
			}  
		}  

		public void sendData(string sData, ref Socket sock)
		{
			message(sData, ref sock, "HTTP/1.1");
		}
		
		public void SendToBrowser(string sData, ref Socket mySocket)  
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

		public void message(string Message, ref Socket mySocket, string sHttpVersion)
		{
			string message = Message;
			SendHeader(sHttpVersion, "", message.Length, "200 OK", ref mySocket);
			SendToBrowser(message , ref mySocket);
		}

		public void error (int code, ref Socket mySocket, string sHttpVersion)
		{
			string message = "Error! <br>Code :" + code;
			SendHeader(sHttpVersion, "", message.Length, "400 Bad Request", ref mySocket);
			SendToBrowser(message , ref mySocket);
		}		
		static void respondToPreflight(ref Socket mySocket)
		{
			string sBuffer = "";  
			sBuffer +=  "HTTP/1.1 204\n";  
			sBuffer += "Connection: keep-alive\n";
			sBuffer += "Access-Control-Allow-Origin: *\n";
			sBuffer += "Access-Control-Allow-Headers: ngrok-skip-browser-warning\n"; 
			sBuffer += "Access-Control-Allow-Methods: GET\n";
			sBuffer += "Access-Control-Max-Age: 86400\n\n";  
			Byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);  
			int numBytes = mySocket.Send(bSendData, bSendData.Length, 0);
			//mySocket.Close();
		}

		public void error (string Message, ref Socket mySocket, string sHttpVersion)
		{
			string message = "Error! <br>" + Message;
			SendHeader(sHttpVersion, "", message.Length, "400 Bad Request", ref mySocket);
			SendToBrowser(message , ref mySocket);
		}

		// request accepting and handling

		public void StartListen() 
		{  
			while (true)  
			{  
				try
				{
				// Listening and server shit

					Console.WriteLine("Accepting A New Client");
					//Accept a new connection  
					Socket mySocket = myListener.AcceptSocket();  

					Thread handlerThread = new Thread(() => handleClient(ref mySocket));
					handlerThread.Start();
				}
				catch(Exception ex)
				{
					Console.WriteLine("error occured, error details : " + ex + "continuing...");
				}
			} 
		}

		public void handleClient(ref Socket mySocket)
		{	
			int iStartPos = 0;  
			string sRequest = "";  		
			TcpClient client = new TcpClient();
			client.Client = mySocket;
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
				if(iStartPos == -1)
				{
					iStartPos = sBuffer.IndexOf("http", 1);
				}
				Console.WriteLine("start Position : " + iStartPos);  
				// Get the HTTP text and version e.g. it will return "HTTP/1.1"  
				string sHttpVersion = sBuffer.Substring(iStartPos, 8);  
				// Extract the Requested Type and Requested file/directory  
				sRequest = sBuffer.Substring(0, iStartPos - 1);  
				//Replace backslash with Forward Slash, if Any  
				sRequest = sRequest.Replace("\\", "/").Replace("%22","'").Replace("%20", " ").Replace("GET /", "").Replace("&",",");

				bool shouldAnswer = true;
				if(sRequest.Contains("OPTIONS /"))
				{
					shouldAnswer=false;
					respondToPreflight(ref mySocket);
				}
				try
				{
					if(shouldAnswer)
					{
						// request handling (in case we did not respond already)
						for(int o = 0; o < ServerFunction.functions.Count; o++)
						{
							if(sRequest.Contains(ServerFunction.functions[o].activation))
							{
								string response = ServerFunction.functions[o].run(sRequest, client); 
								sendData(response, ref mySocket);
							}
						}	
					}
				}
				catch(Exception ex)
				{
					error(ex.Message, ref mySocket, "HTTP/1.1");
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