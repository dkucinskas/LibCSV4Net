using System.IO;
using LibCSV;
using LibCSV.Dialects;
using LibCSV.Exceptions;
using NUnit.Framework;

namespace TestLibCSV
{
	[TestFixture]
	public class AdapterTests
	{
		[Test]
		public void ReadAll_ExistingFileName_ReturnsRecords()
		{
			IDataTransformer transformer = new NullTransformerForAdapterTesting(
				new[] { "Header#1", "Header#2", "Header#3" },
				new[]
				{
					new[] {"1", "2", "3"},
					new[] {"4", "5", "6"}
				});
			
			using (var dialect = new Dialect(true, ';', '"', '\\', true, "\r\n", QuoteStyle.QuoteNone, true, true))
			{
				using (var adapter = new CSVAdapter(dialect, "test.csv", "utf-8"))
				{
					adapter.ReadAll(transformer);
				}
			}	
		}
		
		[Test]
		public void ReadAll_ExistingStream_ReturnsRecords()
		{
			const string input = "Header#1;Header#2;Header#3\r\n1;2;3\r\n4;5;6";
			
			IDataTransformer transformer = new NullTransformerForAdapterTesting(
				new[] { "Header#1", "Header#2", "Header#3" },
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
		public void ReadAll_WithoutHeaders_ReturnRecords()
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
		public void WriteAll_WithoutHeaders_WroteRecords()
		{
			var data = new[] 
			{
				new[] {"1", "2", "3"},
				new[] {"4", "5", "6"}
			};
			IDataTransformer transformer = new NullTransformerForAdapterTesting(
				null, data);
			
			using (var dialect = new Dialect(true, ';', '"', '\\', true, "\r\n", QuoteStyle.QuoteNone, true, false))
			{
				using (var adapter = new CSVAdapter(dialect, new StringWriter()))
				{
					adapter.WriteAll(data, transformer);
				}
			}
		}
		
		[Test]
		public void WriteAll_WithHeader_WroteRecords()
		{
			var headers = new[] { "Header#1", "Header#2", "Header#3" };
			var data = new[] 
			{
				new[] {"1", "2", "3"},
				new[] {"4", "5", "6"}
			};
			IDataTransformer transformer = new NullTransformerForAdapterTesting(
				headers, data);
			
			using (var dialect = new Dialect(true, ';', '"', '\\', true, "\r\n", QuoteStyle.QuoteNone, true, true))
			{
				using (var adapter = new CSVAdapter(dialect, new StringWriter(), headers))
				{
					adapter.WriteAll(data, transformer);
				}
			}
		}
		
		[Test]
		[ExpectedException(typeof(HeaderIsNullException))]
		public void Adapter_HeaderIsNull_ThrowsException()
		{
			IDataTransformer transformer = new NullTransformerForAdapterTesting(new string[] {}, new string[] {});
			
			using (var dialect = new Dialect(true, ';', '"', '\\', true, "\r\n", QuoteStyle.QuoteNone, true, true))
			{
				using (var adapter = new CSVAdapter(dialect, new StringWriter(), null))
				{
				}
			}
		}
		
		[Test]
		[ExpectedException(typeof(DataTransformerIsNullException))]
		public void WriteAll_DataTransformerIsNull_ThrowsException()
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
			
			using (var dialect = new Dialect(true, ';', '"', '\\', true, "\r\n", QuoteStyle.QuoteNone, true, false))
			{
				using (var adapter = new CSVAdapter(dialect, new StringWriter(), headers))
				{
					adapter.WriteAll(data, null);
				}
			}
		}
		
		[Test]
		[ExpectedException(typeof(NotEqualCellCountInRowsException))]
		public void WriteAll_NotEqualCellCountInRows_ThrowsException()
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
				new[] {"4", "5"}
			};
			
			IDataTransformer transformer = new NullTransformerForAdapterTesting(headers, data);
			
			using (var dialect = new Dialect(true, ';', '"', '\\', true, "\r\n", QuoteStyle.QuoteNone, true, false))
			{
				using (var adapter = new CSVAdapter(dialect, new StringWriter(), null))
				{
					adapter.WriteAll(data, transformer);
				}
			}
		}
	}
}

