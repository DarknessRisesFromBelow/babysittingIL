using YMA.Management;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace YMA.Authorization
{

	class Auth
	{
		public uint login(int cluster, int cID, string Username, string Password)
		{
			uint perms = Constants.Visitor;
			AuthAcc acc = ClusterManager.mgr.getUser(cluster, cID);
			if(acc.GetPassword() == Password && acc.GetUsername() == Username)
			{
				perms = acc.perms;
			}

			return perms;
		}
	}

	class AuthAcc : Account
	{
		public int cluster, cID;
		public uint perms;

		public AuthAcc(string Username, string Password, string Email) : base(Username, Password, Email)
		{
			perms = Constants.User;
			
			// temporary values before it gets the real values from function below			
			cluster = 0;
			cID = 0;
			
			ClusterManager.mgr.addUser(this);
		}
	}

	class ClusterManager
	{
		public static ClusterManager mgr = new(3);
		List<Cluster> clusters;

		public ClusterManager(int clusters)
		{
			clusters = new();
			for(int i = 0; i < clusters; i++)
			{
				AddCluster();
			}
		}

		public void AddCluster()
		{
			clusters.Add(new Cluster());
		}

		public void addUser(AuthAcc user)
		{
			Cluster[] free = getFreeClusters();
			if(!(free.Length > 0))
			{
				AddCluster();
				free = getFreeClusters();
			}
			int clusterID = new Random().Next(0, free.Length);
			
			Cluster clstr = free[clusterID];
			int id = clstr.addUser(user);
		
			user.cID = id;
			user.cluster = clusterID;
		}

		Cluster[] getFreeClusters()
		{
			List<Cluster> m_clusters = new();
			Parallel.For(0, this.clusters.Count ,i=>
			{
				if(!this.clusters[i].isFull())
				{
					m_clusters.Add(clusters[i]);
				}
			});
			return m_clusters.ToArray();		
		}

		public AuthAcc getUser(int id, int clusterID)
		{
			return clusters[clusterID].At(id);
		}
	}

	class Cluster
	{
		// TODO: turn to 
		AuthAcc[] arr;
		
		public Cluster()
		{
			arr = new AuthAcc[Constants.UsersPerCluster];
		}

		public int addUser(AuthAcc acc)
		{
			int id = -1;
			if(arr != null && !isFull())
			{
				int[] empty = emptySpots();
				id = new Random().Next(0, empty.Length);
				arr[id] = acc;
			}
			return id;
		}

		public bool isFull() => emptySpots().Length == 0;

		public int[] emptySpots()
		{
			List<int> ids = new();
			Parallel.For(0, this.arr.Length ,i=>
			{
				if(this.arr[i] == null)
				{
					ids.Add(i);
				}
			});			
			return ids.ToArray();
		}

		public AuthAcc At(int index) => arr[index];
	}

	class Constants
	{
		// set user pfp for your id
		// set user pfp for other ids
		// change email for any user
		// change email for your user
		// change password for any user
		// change password for your user
		// change username for your user
		// change username for any user
		// 11111111
		public const uint Admin = 0xFF;
		
		// 10010110
		public const uint User = 0x96;

		// 00000000
		public const uint Visitor = 0x00;

		public const uint UsersPerCluster = 10000;

	}
}