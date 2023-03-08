using babysittingIL.UserManagement;

namespace babysittingIL.reviewManagement
{
	public struct review
	{
		int stars;
		string Review;
		string fromUsername;
		int fromID;
		int toID;

		public review(int Stars, string reviewString, int commentedUserID, int fromUserID)
		{
			stars = Stars;
			Review = reviewString;
			toID = commentedUserID;
			fromID = fromUserID;
			user accref = user.GetUserByID(fromID);
			fromUsername = accref.GetUsername();
		}

		public string getReview()
		{
			return "" + fromID + "," + fromUsername +"," + toID + "," + stars + "," + Review;
		}
	}
}