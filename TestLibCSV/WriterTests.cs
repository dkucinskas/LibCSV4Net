using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

using LibCSV;
using LibCSV.Dialects;
using LibCSV.Exceptions;
using NUnit.Framework;

namespace TestLibCSV
{
	[TestFixture]
	public class WriterTests
	{
		[Test]
		public void WriteRow_Numbers_WroteNumbers()
		{
			WriteAndTestRow(
				new object[] { 123, 123.45, 10M, 1 },
				"123;123.45;10;1\r\n", null);
		}
		
		[Test]
		public void WriteRow_Nulls_WroteEmptyStrings()
		{
			WriteAndTestRow(new object[] { null, null }, ";\r\n", null);
		}
		
		[Test]
		public void WriteRow_Strings_WroteStrings()
		{
			WriteAndTestRow(
				new object[] { "This is string1", "This is string2" },
				"\"This is string1\";\"This is string2\"\r\n", null);
		}
		
		[Test]
		public void WriteRow_Dates_WroteDates()
		{
			WriteAndTestRow(
				new object[] 
				{
					new DateTime(2010, 9, 3, 0, 0, 0), 
					new DateTime(2010, 9, 4, 0, 0, 0) 
				}, 
				"09/03/2010 00:00:00;09/04/2010 00:00:00\r\n", null);
		}
		
		[Test]
		public void WriteRow_QuoteAll_Quoted()
		{
			var row = new object[]
			{
				1, 2, 3, new DumyObject(4)
			};
			
			string results;
			using (var writer = new StringWriter())
			{
				var dialect = new Dialect(true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteAll, false, false);
				using (var csvWriter = new CSVWriter(dialect, writer))
				{
					csvWriter.WriteRow(row);
				}
				
				results = writer.ToString();
			}
			
			Assert.AreEqual("\"1\";\"2\";\"3\";\"4\"\r\n", results);
		}
		
		[Test]
		public void WriteRow_EscapeStrings_Escaped()
		{
			var row = new object[] { "\"" };
			
			string results;
			using (var writer = new StringWriter())
			{
				var dialect = new Dialect(true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteAll, false, false);
				using (var csvWriter = new CSVWriter(dialect, writer))
				{
					csvWriter.WriteRow(row);
				}
				
				results = writer.ToString();
			}
			
			Assert.AreEqual("\"\\\"\"\r\n", results);
		}
		
		[Test]
		public void WriteRow_DoNotEscapeStrings_NotEscaped()
		{
			var row = new object[] { "s" };
			
			string results;
			using (var writer = new StringWriter())
			{
				var dialect = new Dialect(false, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteAll, false, false);
				using (var csvWriter = new CSVWriter(dialect, writer))
				{
					csvWriter.WriteRow(row);
				}
				
				results = writer.ToString();
			}
			
			Assert.AreEqual("\"s\"\r\n", results);
		}
		
		[Test]
		[ExpectedException(typeof(DialectIsNullException))]
		public void ConstructorFirst_DialectIsNull_ThrowsDialectIsNullException()
		{
			using (var writer = new CSVWriter(null, Guid.NewGuid().ToString(), "UTF-8"))
			{
			}
		}
		
		[Test]
		[ExpectedException(typeof(DialectIsNullException))]
		public void ConstructorSecond_DialectIsNull_ThrowsDialectIsNullException()
		{
			using (var stringWriter = new StringWriter())
			{
				using (var writer = new CSVWriter(null, stringWriter))
				{
				}
			}
		}
		
		[Test]
		[ExpectedException(typeof(FileNameIsNullOrEmptyException))]
		[TestCase(null)]
		[TestCase("")]
		[TestCase("  ")]
		public void Constructor_FileNameIsNullOrEmpty_ThrowsFileNameIsNullOrEmptyException(string fileName)
		{
			using (var dialect = new Dialect(true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				using (var writer = new CSVWriter(dialect, fileName, "UTF-8"))
				{
				}
			}
		}
		
		[Test]
		[ExpectedException(typeof(CannotWriteToFileException))]
		public void Constructor_FileNotExists_ThrowsCannotWriteToFileException()
		{
			using (var dialect = new Dialect(true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				using (var writer = new CSVWriter(dialect, "VeryLongNonExistingFileNameForTesting", "UTF-8"))
				{
				}
			}
		}
		
		[Test]
		[ExpectedException(typeof(CannotWriteToFileException))]
		public void Constructor_FileIsLocked_ThrowsCannotWriteToFileException()
		{
			using (var writer = new StreamWriter("test_write_file_locked.csv", false, Encoding.GetEncoding("utf-8")))
			{
				using (var dialect = new Dialect(true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
				{
					using (var csvWriter = new CSVWriter(dialect, "test_write_file_locked.csv", "UTF-8"))
					{
					}
				}
			}
		}
		
		[Test]
		[ExpectedException(typeof(RowIsNullOrEmptyException))]
		public void WriteRow_RowIsNull_ThrowsRowIsNullOrEmptyException()
		{
			using (var stringWriter = new StringWriter())
			{
				using (var dialect = new Dialect(true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
				{
					using (var writer = new CSVWriter(dialect, stringWriter))
					{
						writer.WriteRow(null);
					}
				}
			}
		}
		
		internal class DumyObject
		{
			public DumyObject(int number)
			{
				Number = number;
			}
			
			public int Number { get; set; }
			
			public override string ToString()
			{
				return Number.ToString();
			}
		}
		
		private void WriteAndTestRow(object[] input, string output, Dialect dialect)
		{
			dialect = dialect ?? new Dialect(
				true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteMinimal, false, false);
			
			var oldCulture = Thread.CurrentThread.CurrentCulture;
			
			try
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
				
				string results;
				using (var writer = new StringWriter())
				{
					using (var csvWriter = new CSVWriter(dialect, writer))
					{
						csvWriter.WriteRow(input);
					}
					
					results = writer.ToString();
				}
				
				Assert.AreEqual(output, results);
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = oldCulture;
			}
		}
	}
}

