using babysittingIL.UserManagement;
using babysittingIL.UserManagement.location;

namespace babysittingIL.UserExperience
{
	public class HomeManager
	{
		public string GetUserHome(int id)
		{	
			string users = "";
			user myUser = user.GetUserByID(id);
			for(int i = 0; i < user.users.Count; i++)
			{
				user secondUser = user.GetUserByID(i);
				float dist = locationObject.getDistance(myUser.getLocation(),secondUser.getLocation());
				int radius = 2500;
				Console.WriteLine("the distance is "+ dist + "and the range is "+ radius + ", which means the value of dist<radius will be "+ (dist<radius));
				if(i != id && dist<radius)
				{
					if(i == user.users.Count)
						users += user.users[i].GetData();
					else
						users += user.users[i].GetData() +"|||";
				}
			}
			return users;
		}
	}
}