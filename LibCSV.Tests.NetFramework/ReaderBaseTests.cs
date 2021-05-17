using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LibCSV.Dialects;
using NUnit.Framework;

namespace LibCSV.Tests
{
	public class ReaderBaseTests
	{
		public static void DisposeIListOfIListOfObjects(IList<IList<object>> list)
		{
			if (list != null && list.Count > 0)
			{
				var count = list.Count;
				for (var i = 0; i < count; i++)
				{
					if (list[i] != null && list[i].Count > 0)
					{
					    for (var j = 0; j < count; j++)
						{
							if (list[i][j] != null)
							{
								var disposable = list[i][j] as IDisposable;
								if (disposable != null)
								{
									disposable.Dispose();
								}
							}
							list[i][j] = null;
						}
					}
					list[i] = null;
				}
			}
		}
		
		public static void ReadTest(string input, IList<IList<object>> expect, Dialect dialect)
		{
			IList<IList<object>> results = new List<IList<object>>();
			using (var reader = new CSVReader(dialect, new StringReader(input)))
			{
				reader.Open();

				while (reader.Next())
				{
					var record = reader.Current;
					if (record != null && record.Length > 0) 
					{
						results.Add (record);
					}
				}
			}
			
			Assert.AreEqual(expect, results);
			
			DisposeIListOfIListOfObjects(results);
			DisposeIListOfIListOfObjects(expect);
		}

		public static async Task ReadTestAsync(string input, IList<IList<object>> expect, Dialect dialect)
		{
			IList<IList<object>> results = new List<IList<object>>();
			using (var reader = new CSVReader(dialect, new StringReader(input)))
			{
				await reader.OpenAsync();

				while (await reader.NextAsync())
				{
					var record = reader.Current;
					if (record != null && record.Length > 0)
					{
						results.Add(record);
					}
				}
			}

			Assert.AreEqual(expect, results);

			DisposeIListOfIListOfObjects(results);
			DisposeIListOfIListOfObjects(expect);
		}
	}
}
