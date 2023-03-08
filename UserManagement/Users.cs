using YMA.Management;
using babysittingIL.Constants;
using babysittingIL.Messaging;
using babysittingIL.UserManagement.location;
using babysittingIL.sessionManagement;
using babysittingIL.reviewManagement;
using System.Net;

namespace babysittingIL.UserManagement
{
	enum UserType {

		Babysitter,
		Parent
	}

	public class user
	{
		UserType userType;
		Account acc;
		public string profilePicURL;
		public int score = 0;
		public int reviews = 0;
		public float rating = 0.0f;
		float rate = 25.7f;
		locationObject location;
		public static List<user> users = new();
		bool LoggedIn = false;
		string sessionID = "";
		string bio = "";



		public user(int Type, string name, string email, string password, string pfpURL = Consts.defaultPfpURL)
		{	
			profilePicURL = pfpURL;
			userType = Type == 1 ? UserType.Babysitter : UserType.Parent ;
			acc = RegistrationManager.CreateAccount(name, password,email);
			location = new((1,0));
			MessagingManager.Lists.Add(new());
			users.Add(this);
			reviewManager.AddUser(GetID());
			if(userType == UserType.Babysitter)
			{
				SendMail("welcome to BabysittingIL!", Consts.BabysitterWelcomeMessage);
			}
			else
			{
				SendMail("welcome to BabysittingIL!", Consts.ParentWelcomeMessage);
			}
		}

		////////////////////////////// obselete //////////////////////////////
		public void AddScore(int scoreToAdd, int fromID)
		{
			score += scoreToAdd;
			reviews++;
			rating = score/reviews;
		}
		public void AddScoreRelative(int scoreToAdd, int fromID)
		{
			user targetUser = user.GetUserByID(fromID);
			score += targetUser.GetRating()  >= 2 ? (int)(scoreToAdd / (6 - targetUser.GetRating())) : (int)(scoreToAdd / 4);
			reviews++;
			rating = score/reviews;
		}
		///////////////////////////////////////////////////////////////////////

		public string GetEmail() => acc.GetEmailAddress();
		public string GetUsername() => acc.GetUsername();
		public string GetPassword() => acc.GetPassword();
		public string getSessionID() => sessionID;
		public void SendMail(string header, string content) => acc.SendMail(Consts.Header + header, content);
		public static user NotAUser = new(1,"NotAUser","unidentified123.randomail@gmail.com","allo");
		public int GetID() => acc.GetID();
		public float GetRating() => rating;
		public string GetPFP() => profilePicURL;
		public void SetPFP(string newURL) => profilePicURL = newURL;
		public float GetRate() => rate;
		public void SetRate(float newRate)	=> rate = newRate;
		public string GetBio() => bio;
		public void SetBio(string newBio) => bio = newBio;
		public string GetReviews() => reviewManager.getReviews(GetID()); 
		public (double,double) getLocation() => locationManager.locations[GetID()].getLocation();
		public void setLocation((double,double) nLocation) => locationManager.locations[GetID()].setLocation(nLocation);
		public static int MakeNew(string options)
		{
			try
			{
				string[] retrived = options.Split(',');
				Console.WriteLine("Retrived Info : " + retrived[0]+","+ retrived[1]+","+ retrived[2]+","+ retrived[3]);
				if(retrived.Length > 4)
				{
					return new user(int.Parse(retrived[0]),retrived[1], retrived[2], retrived[3], retrived[4]).GetID();
				}
				else
				{
					return new user(int.Parse(retrived[0]),retrived[1], retrived[2], retrived[3]).GetID();
				}
			}
			catch(Exception ex)
			{
				users.RemoveAt(users.Count);
				Console.WriteLine(ex);
				throw new Exception("Couldnot create user. presumably due to duplicate email or a username that already exists in the system");
				return -1;
			}
		}

		public void AddReview(int fromID, string comment, int rating)
		{
			reviewManager.AddReview(GetID(), fromID, rating, comment);
		}

		public bool login(string password, EndPoint connectedDevice)
		{
			if(password == GetPassword())
			{
				LoggedIn = true;
				sessionID = new session(this,connectedDevice).getSessionID();
				return true;
			}
			else
			{
				throw new Exception("could not log user in. either username or password are incorrect");
				return false;
			}
		}

		public static int GetIdByName(string name)
		{
			for(int i = 0; i < GeneralManagement.activeAccounts.Count; i++)
			{
				if(GeneralManagement.activeAccounts[i].GetUsername() == name)
				{
					return GeneralManagement.activeAccounts[i].GetID();
				}
			}
			return -1;
		}
		public static user GetUserByID(int id)
		{
			for(int i = 0; i < GeneralManagement.activeAccounts.Count; i++)
			{
				if(GeneralManagement.activeAccounts[i].GetID() == id)
				{
					return users[i];
				}
			}
			return NotAUser;
		}
		public string GetData() => "" + GetUsername() + "," + GetRating() + "," + GetPFP() +","+ GetBio() + "," + GetRate() + "," + GetID();
	}
}