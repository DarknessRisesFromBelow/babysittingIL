using System.Collections.Generic;

namespace babysittingIL.reviewManagement
{
	public class reviewManager
	{
		public static List<List<review>> reviews = new List<List<review>>();

		public static void AddReview(int id, int fromID, int stars, string reviewString)
		{
			reviews[id].Add(new review(stars, reviewString, id, fromID));
		}

		public static string getReviews(int id)
		{
			string reviewsStr = "";
			for(int i = 0; i < reviews[id].Count; i++)
			{
				reviewsStr += reviews[id][i].getReview();
			}
			return reviewsStr;
		}
	}
}	