using babysittingIL.UserManagement;

namespace babysittingIL.reviewManagement
{
	// Represents a review of a user, including a star rating, a review text,
	// and information about the user who left the review and the user who was reviewed.
	public struct review
	{
   	 	// The number of stars given in the review (1-5).
    	int stars;

    	// The text of the review.
    	string Review;

    	// The username of the user who left the review.
    	string fromUsername;

    	// The ID of the user who left the review.
    	int fromID;

    	// The ID of the user who was reviewed.
    	int toID;

    	// Constructs a new review object with the given star rating, review text,
    	// and user IDs for the user who left the review and the user who was reviewed.
    	public review(int Stars, string reviewString, int commentedUserID, int fromUserID)
    	{
      	  	stars = Stars;
        	Review = reviewString;
        	toID = commentedUserID;
        	fromID = fromUserID;

        	// Get the user object for the user who left the review.
        	user accref = user.GetUserByID(fromID);

        	// Get the username for the user who left the review.
        	fromUsername = accref.GetUsername();
    	}

    	// Returns a string representation of the review in the format:
    	// "<fromID>,<fromUsername>,<toID>,<stars>,<Review>"
    	public string getReview()
    	{
        	return "" + fromID + "," + fromUsername +"," + toID + "," + stars + "," + Review;
    	}
	}
}