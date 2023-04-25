using System.Linq;
using System.Collections.Generic;

namespace babysittingIL.UserManagement.location
{
	public class locationObject
	{
		(double, double) location;
		public locationObject((double, double) Location)
		{
			location = Location;
			locationManager.locations.Add(this);
		}
		public (double,double) getLocation() => location;
		public double getLongitude() => location.Item1;
		public double getLatitude() => location.Item2;
		public void setLocation((double,double) nLocation) => location=nLocation;
		public void setLongitude(int longitude) => location = (longitude, location.Item2);
		public void setLatitude(int latitude) => location = (location.Item1, latitude);
		
		public override string ToString()
		{
			return "{" + getLongitude() + "," + getLatitude() + "}";
		}

		public static float getDistance((double,double) pos1, (double,double) pos2)
		{
			var R = 6378.137f;
			var dLat =  pos2.Item2 * Math.PI / 180 - pos1.Item2 * Math.PI / 180;
			var dLon =  pos2.Item1 * Math.PI / 180 - pos1.Item1 * Math.PI / 180;
   			var a = Math.Sin(dLat/2) * Math.Sin(dLat/2) + Math.Cos(pos1.Item2 * Math.PI / 180) * Math.Cos(pos2.Item2 * Math.PI / 180) * Math.Sin(dLon/2) * Math.Sin(dLon/2);
			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
			var d = R * c;
			return (float) d * 1000;
		}
	}

	class locationManager
	{
		public static List<locationObject> locations = new();
	}
}