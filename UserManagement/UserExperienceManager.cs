using babysittingIL.UserManagement;
using babysittingIL.UserManagement.location;
using babysittingIL.Constants;
using System.Threading;
using System.Threading.Tasks;

namespace babysittingIL.UserExperience
{
	public class HomeManager
	{
		public string GetUserHome(int id)
		{	
			string users = "";
			user myUser = user.GetUserByID(id);
			Parallel.For(0, user.users.Count, i=>
			{
				user secondUser = user.GetUserByID(i);
				float dist = locationObject.getDistance(myUser.getLocation(),secondUser.getLocation());
				int radius = Consts.Dist;
				Console.WriteLine("the distance is "+ dist + "and the range is "+ radius + ", which means the value of dist<radius will be "+ (dist<radius));
				if(i != id && dist<radius && secondUser.GetType() == 1)
				{
					if(i == user.users.Count)
						users += user.users[i].GetData();
					else
						users += user.users[i].GetData() +"|||";
				}
			});
			return users;
		}
	}
}