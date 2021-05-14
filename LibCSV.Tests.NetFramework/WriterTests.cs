using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibCSV;
using LibCSV.Dialects;
using LibCSV.Exceptions;
using NUnit.Framework;

using static NUnit.Framework.Assert;

namespace LibCSV.Tests
{
	[TestFixture]
	public class WriterTests
	{
		[Test]
		public void WriteRow_Convertibles_WroteConvertibles()
		{
			WriteAndTestRow(
				new object[] { 123, 123.45, 10M, 1, new DateTime(2015, 10, 26, 10, 11, 12) },
				"123;123.45;10;1;10/26/2015 10:11:12\r\n", null);
		}

		[Test]
		public async Task WriteRowAsync_Convertibles_WroteConvertibles()
		{
			await WriteAndTestRowAsync(
				new object[] { 123, 123.45, 10M, 1, new DateTime(2015, 10, 26, 10, 11, 12) },
				"123;123.45;10;1;10/26/2015 10:11:12\r\n", null);
		}

		[Test]
		public void WriteRow_Nulls_WroteEmptyStrings()
		{
			WriteAndTestRow(new object[] { null, null }, ";\r\n", null);
		}

		[Test]
		public async Task WriteRowAsync_Nulls_WroteEmptyStrings()
		{
			await WriteAndTestRowAsync(new object[] { null, null }, ";\r\n", null);
		}

		[Test]
		public void WriteRow_Strings_WroteStrings()
		{
			WriteAndTestRow(
				new object[] { "This is string1", "This is string2" },
				"\"This is string1\";\"This is string2\"\r\n", null);
		}

		[Test]
		public async Task WriteRowAsync_Strings_WroteStrings()
		{
			await WriteAndTestRowAsync(
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
		public async Task WriteRowAsync_Dates_WroteDates()
		{
			await WriteAndTestRowAsync(
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
			
			AreEqual("\"1\";\"2\";\"3\";\"4\"\r\n", results);
		}

		[Test]
		public async Task WriteRowAsync_QuoteAll_Quoted()
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
					await csvWriter.WriteRowAsync(row);
				}

				results = writer.ToString();
			}

			AreEqual("\"1\";\"2\";\"3\";\"4\"\r\n", results);
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
			
			AreEqual("\"\\\"\"\r\n", results);
		}

		[Test]
		public async Task WriteRowAsync_EscapeStrings_Escaped()
		{
			var row = new object[] { "\"" };

			string results;
			using (var writer = new StringWriter())
			{
				var dialect = new Dialect(true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteAll, false, false);
				using (var csvWriter = new CSVWriter(dialect, writer))
				{
					await csvWriter.WriteRowAsync(row);
				}

				results = writer.ToString();
			}

			AreEqual("\"\\\"\"\r\n", results);
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
			
			AreEqual("\"s\"\r\n", results);
		}

		[Test]
		public async Task WriteRowAsync_DoNotEscapeStrings_NotEscaped()
		{
			var row = new object[] { "s" };

			string results;
			using (var writer = new StringWriter())
			{
				var dialect = new Dialect(false, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteAll, false, false);
				using (var csvWriter = new CSVWriter(dialect, writer))
				{
					await csvWriter.WriteRowAsync(row);
				}

				results = writer.ToString();
			}

			AreEqual("\"s\"\r\n", results);
		}

		[Test]
		public void ConstructorFirst_DialectIsNull_ThrowsDialectIsNullException()
		{
            Throws<DialectIsNullException>(() =>
            {
                using (var writer = new CSVWriter(null, Guid.NewGuid().ToString(), "UTF-8"))
                {
                }
            });
		}

		[Test]
		public void ConstructorSecond_DialectIsNull_ThrowsDialectIsNullException()
		{
            Throws<DialectIsNullException>(() =>
            {
                using (var stringWriter = new StringWriter())
                {
                    using (var writer = new CSVWriter(null, stringWriter))
                    {
                    }
                }
            });
		}
		
		[Test]
		[TestCase(null)]
		[TestCase("")]
		[TestCase("  ")]
		public void Constructor_FileNameIsNullOrEmpty_ThrowsFileNameIsNullOrEmptyException(string fileName)
		{
            Throws<FileNameIsNullOrEmptyException>(() =>
            {
                using (var dialect = new Dialect(true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
                {
                    using (var writer = new CSVWriter(dialect, fileName, "UTF-8"))
                    {
                    }
                }
            });
		}
		
		[Test]
		public void Constructor_FileNotExists_ThrowsCannotWriteToFileException()
		{
            Throws<CannotWriteToFileException>(() =>
            {
                using (var dialect = new Dialect(true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
                {
                    using (var writer = new CSVWriter(dialect, "VeryLongNonExistingFileNameForTesting", "UTF-8"))
                    {
                    }
                }
            });
		}
		
		[Test]
		public void Constructor_FileIsLocked_ThrowsCannotWriteToFileException()
		{
            Throws<CannotWriteToFileException>(() =>
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
            });
		}
		
		[Test]
		public void WriteRow_RowIsNull_ThrowsRowIsNullOrEmptyException()
		{
           Throws<RowIsNullOrEmptyException>(() =>
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
           });
		}

		[Test]
		public void WriteRowAsync_RowIsNull_ThrowsRowIsNullOrEmptyException()
		{
			ThrowsAsync<RowIsNullOrEmptyException>(async () =>
			{
				using (var stringWriter = new StringWriter())
				{
					using (var dialect = new Dialect(true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
					{
						using (var writer = new CSVWriter(dialect, stringWriter))
						{
							await writer.WriteRowAsync(null);
						}
					}
				}
			});
		}

		[Test]
		public void WriteRow_ConvertiblesWithSpecifiedCulture_WroteConvertibles()
		{
			var ltCultureInfo = CultureInfo.GetCultureInfo("lt-LT");

			WriteAndTestRow(
				new object[] { 123, 123.45, 10M, 1, new DateTime(2015, 10, 26, 10, 11, 12) },
				"123;123,45;10;1;2015-10-26 10:11:12\r\n", null, ltCultureInfo);
		}

		[Test]
		public async Task WriteRowAsync_ConvertiblesWithSpecifiedCulture_WroteConvertibles()
		{
			var ltCultureInfo = CultureInfo.GetCultureInfo("lt-LT");

			await WriteAndTestRowAsync(
				new object[] { 123, 123.45, 10M, 1, new DateTime(2015, 10, 26, 10, 11, 12) },
				"123;123,45;10;1;2015-10-26 10:11:12\r\n", null, ltCultureInfo);
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
		
		private void WriteAndTestRow(object[] input, string output, Dialect dialect, CultureInfo culture = null)
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
					using (var csvWriter = new CSVWriter(dialect, writer, culture))
					{
						csvWriter.WriteRow(input);
					}
					
					results = writer.ToString();
				}
				
				AreEqual(output, results);
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = oldCulture;
			}
		}

		private async Task WriteAndTestRowAsync(object[] input, string output, Dialect dialect, CultureInfo culture = null)
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
					using (var csvWriter = new CSVWriter(dialect, writer, culture))
					{
						await csvWriter.WriteRowAsync(input);
					}

					results = writer.ToString();
				}

				AreEqual(output, results);
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = oldCulture;
			}
		}
	}
}

