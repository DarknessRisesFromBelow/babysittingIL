using babysittingIL.UserManagement;
using System.IO;

namespace babysittingIL.storingManagement
{
	class storingManager
	{
		public static void saveUser(user userToSave)
		{
			string path = consts.FILEPATH + consts.USER_DB_FILENAME;
			string userData = string.Format(consts.USER_DB_FORMAT, userToSave.GetEmail(), userToSave.GetID());

			if (!File.Exists(path))
			{
				(new FileInfo(path)).Directory.Create();
				File.WriteAllText(path, userData);
			}
			else
			{
				File.AppendAllText(path, userData);
			}
		}
	}
}