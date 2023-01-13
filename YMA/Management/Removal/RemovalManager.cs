namespace YMA.Management
{
	public class RemovalManager
	{
		public static void RemoveAccount(int UserID)
		{
			GeneralManagement.activeAccounts.RemoveAt(UserID - 1);
		}
	}
}