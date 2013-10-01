using System;
using System.Collections;
using System.IO;
using LibCSV;
using LibCSV.Dialects;
using NUnit.Framework;

namespace TestLibCSV
{
	[TestFixture]
	public class AdapterTests
	{
		internal class NullTransformerForAdapterTesting : IDataTransformer
		{
			private string[] _aliases;
			private IList _records;
			private int _row;

			public NullTransformerForAdapterTesting(string[] expectedAliases, IList expectedResults)
			{
				_aliases = expectedAliases;
				_records = expectedResults;
			}

			public object TransformTuple(object[] tuple, string[] aliases)
			{
				if (_aliases != null)
					Assert.AreEqual(_aliases, aliases);

				if (_records != null)
					Assert.AreEqual(_records[_row], tuple);

				_row++;
				return null;
			}

			public IEnumerable TransformResult(IEnumerable result)
			{
				return null;
			}

			public object[] TransformRow(object tuple)
			{
				if (_records != null)
				{
					Assert.AreEqual(_records[_row], tuple);
					_row++;
				}

				return null;
			}
		}

		[Test]
		public void ReadAll_InputWithHeader_Ok()
		{
			const string input = "Header#1;Header#2;Header#3\r\n1;2;3\r\n4;5;6";

			IDataTransformer transformer = new NullTransformerForAdapterTesting(
			new[] 
			{ 
				"Header#1", 
				"Header#2", 
				"Header#3" 
			},
			new[] 
			{
				new[] {"1", "2", "3"},
				new[] {"4", "5", "6"}
			});

			using (var dialect = new Dialect(true, ';', '"', '\\', true, "\r\n", QuoteStyle.QuoteNone, true, true))
			{
				using (var adapter = new CSVAdapter(dialect, new StringReader(input)))
				{
					adapter.ReadAll(transformer);
				}
			}
		}

		[Test]
		public void ReadAll_InputWithDateTime_Ok()
		{
			const string input = "Date;Header#2;Header#3\r\n00:15:00;2;3\r\n00:20:10;5;6";

			IDataTransformer transformer = new NullTransformerForAdapterTesting(
			new[]
			{
				"Date", 
				"Header#2", 
				"Header#3" 
			},
			new[]
			{
				new object[] {TimeSpan.FromMinutes(15), 2L, "3"},
				new object[] {new TimeSpan(0, 20, 10), 5L, "6"}
			});

			using (var dialect = new Dialect(true, ';', '"', '\\', true, "\r\n", QuoteStyle.QuoteNone, true, true))
			{
				using (var adapter = new CSVAdapter(dialect, new StringReader(input)))
				{
					adapter.SetColumn("Date", typeof(TimeSpan));
					adapter.SetColumn("Header#2", typeof(long));
					adapter.SetColumn("Header#3", typeof(string));
					adapter.ReadAll(transformer);
				}
			}
		}

		[Test]
		public void ReadAll_InputWithoutHeader_Ok()
		{
            const string input = "1;2;3\r\n4;5;6";

			IDataTransformer transformer = new NullTransformerForAdapterTesting(
			null,
			new[] 
			{
				new[] {"1", "2", "3"},
				new[] {"4", "5", "6"}
			});

			using (var dialect = new Dialect(true, ';', '"', '\\', true, "\r\n", QuoteStyle.QuoteNone, true, false))
			{
				using (var adapter = new CSVAdapter(dialect, new StringReader(input)))
				{
					adapter.ReadAll(transformer);
				}
			}
		}

		[Test]
		public void WriteAll_OutputWithoutHeader_Ok()
		{
            var data = new[] 
				{
					new[] {"1", "2", "3"},
					new[] {"4", "5", "6"}
				};

			IDataTransformer transformer = new NullTransformerForAdapterTesting(
				null,
				data);

			using (var dialect = new Dialect(true, ';', '"', '\\', true, "\r\n", QuoteStyle.QuoteNone, true, false))
			{
				using (var adapter = new CSVAdapter(dialect, new StringWriter()))
				{
					adapter.WriteAll(data, transformer);
				}
			}
		}

		[Test]
		public void WriteAll_OutputWithHeader_Ok()
		{
			var headers = new[] 
			{ 
				"Header#1", 
				"Header#2", 
				"Header#3" 
			};

			var data = new[] 
			{
				new[] {"1", "2", "3"},
				new[] {"4", "5", "6"}
			};

			IDataTransformer transformer = new NullTransformerForAdapterTesting(headers, data);

			using (var dialect = new Dialect(true, ';', '"', '\\', true, "\r\n", QuoteStyle.QuoteNone, true, true))
			{
                using (var adapter = new CSVAdapter(dialect, new StringWriter(), headers))
				{
					adapter.WriteAll(data, transformer);
				}
			}
		}

		[Test]
		[ExpectedException(typeof(DataTransformerIsNullException))]
		public void WriteAll_ShouldThrowDataTransformerIsNullException_Ok()
		{
			var data = new[] 
			{
				new[] {"1", "2", "3"},
				new[] {"4", "5", "6"}
			};

			using (var dialect = new Dialect(true, ';', '"', '\\', true, "\r\n", QuoteStyle.QuoteNone, true, false))
			{
				using (var adapter = new CSVAdapter(dialect, new StringWriter(), null))
				{
					adapter.WriteAll(data, null);
				}
			}
		}
	}
}

