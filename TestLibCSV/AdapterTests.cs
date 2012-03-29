using System;
using NUnit.Framework;
using LibCSV.Dialects;
using LibCSV;
using System.IO;
using System.Collections;

namespace TestLibCSV
{
	[TestFixture]
	public class AdapterTests
	{
		internal class NullTransformerForAdapterTesting : IResultTransformer
		{
			private string[] _aliases = null;
			private IList _records = null;
			private int row = 0;
			
			public NullTransformerForAdapterTesting(string[] expectedAliases, IList expectedResults)
			{
				_aliases = expectedAliases;
				_records = expectedResults;
			}
				
			public object TransformTuple (object[] tuple, string[] aliases)
			{
				if (_aliases != null)
					Assert.AreEqual(_aliases, aliases);
				
				if (_records != null)
					Assert.AreEqual(_records[row], tuple);
				
				row++;
				return null;
			}

			public IEnumerable TransformResult (IEnumerable result)
			{
				return null;
			}
		}
		
		public AdapterTests ()
		{		
		}
		
		[Test]
		public void ReadAll_InputWithHeader_Ok()
		{
			string input = "Header#1;Header#2;Header#3\r\n1;2;3\r\n4;5;6";
			
			IResultTransformer transformer = new NullTransformerForAdapterTesting(
			new string[] 
			{ 
				"Header#1", 
				"Header#2", 
				"Header#3" 
			},
			new ArrayList() 
			{
				new string[] {"1", "2", "3"},
				new string[] {"4", "5", "6"},
			});
			
			using (Dialect dialect = new Dialect(true, ';', '"', '\\', true, "\r\n", QuoteStyle.QUOTE_NONE, true, true))
			{
				using (CSVAdapter adapter = new CSVAdapter(dialect, new StringReader(input)))
				{
					adapter.ReadAll(transformer);
				}
			}
		}
		
		[Test]
		public void ReadAll_InputWithoutHeader_Ok()
		{
			string input = "1;2;3\r\n4;5;6";
			
			IResultTransformer transformer = new NullTransformerForAdapterTesting(
			null,
			new ArrayList() 
			{
				new string[] {"1", "2", "3"},
				new string[] {"4", "5", "6"},
			});
			
			using (Dialect dialect = new Dialect(true, ';', '"', '\\', true, "\r\n", QuoteStyle.QUOTE_NONE, true, false))
			{
				using (CSVAdapter adapter = new CSVAdapter(dialect, new StringReader(input)))
				{
					adapter.ReadAll(transformer);
				}
			}
		}
	}
}

