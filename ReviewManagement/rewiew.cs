namespace babysittingIL.reviewManagement
{
	public struct review
	{
		int stars;
		string Review;
		int fromID;
		int toID;

		public review(int Stars, string reviewString, int commentedUserID, int fromUserID)
		{
			stars = Stars;
			Review = reviewString;
			toID = commentedUserID;
			fromID = fromUserID;
		}

		public string getReview()
		{
			return "" + fromID + "," + toID + "," + stars + "," + Review;
		}
	}
}