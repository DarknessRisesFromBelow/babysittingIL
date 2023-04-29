namespace babysittingIL.sorting
{
	
	class algorithms
	{

		/// <summary>
		/// function sorts the arr using the gnome sort sorting algorithm
		/// </summary>
		///
		/// <param name="arr">The arr</param>
		/// <param name="n">number of elements in the arr</param>
		public static void gnomeSort(string[] arr, int n)
		{
			int index = 0;

			while (index < n) 
			{
				if (index == 0)
				index++;
				if (ulong.Parse(arr[index].Split(" : ")[4]) >= ulong.Parse(arr[index - 1].Split(" : ")[4]))
				index++;
				else {
					string temp = arr[index];
					arr[index] = arr[index - 1];
					arr[index - 1] = temp;
					index--;
				}
			}
		}


		/// <summary>
		/// sorts the arr in a stupid way that doesn't work, multithreaded though so idrc it's fast
		/// </summary>
		///
		/// <param name="messagesArr">The messages arr</param>
		public static void stupidSortThatDoesNotWork(string[] messagesArr)
		{
			Parallel.For(0, messagesArr.Length * messagesArr.Length, i=>
			{
				i = i % (messagesArr.Length - 1);
				if(i > 0 && ulong.Parse(messagesArr[i].Split(":")[4]) > ulong.Parse(messagesArr[i - 1].Split(":")[4]))
				{
					string temp = messagesArr[i-1];
					messagesArr[i-1] = messagesArr[i];
					messagesArr[i] = temp;
				}
			});
		}

	}
}