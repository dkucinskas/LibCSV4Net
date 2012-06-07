using System.Collections.Generic;
using System.IO;
using LibCSV;
using LibCSV.Dialects;
using LibCSV.Exceptions;
using NUnit.Framework;

namespace TestLibCSV
{
	[TestFixture]
	public class ReaderTests : ReaderBaseTests
	{
		[Test]
		[ExpectedException(typeof(TextReaderIsNullException))]
		public void Reader_NullStream_ThrowException()
		{
			IList<IList<object>> results = new List<IList<object>>();

			using (var dialect = new Dialect())
			{
				using (var reader = new CSVReader(dialect, null))
				{
					while (reader.NextRecord())
					{
						var record = reader.GetCurrentRecord();
						if (record != null && record.Length > 0)
							results.Add(record);
					}
				}
			}
		}

		[Test]
		[ExpectedException(typeof(DialectIsNullException))]
		public void Reader_NullDialect_ThrowException()
		{
			IList<IList<object>> results = new List<IList<object>>();

			using (var reader = new CSVReader(null, new StringReader("1,2,3")))
			{
				while (reader.NextRecord())
				{
					var record = reader.GetCurrentRecord();
					if (record != null && record.Length > 0)
						results.Add(record);
				}
			}
		}

		[Test]
		[ExpectedException(typeof(DialectInternalErrorException))]
		public void Reader_DialectInternalError_ThrowException()
		{
			IList<IList<object>> results = new List<IList<object>>();

			using (var dialect = new Dialect(true, '\0', '"', '\\', true, "\r\n", QuoteStyle.QuoteMinimal, true, false))
			{
				using (var reader = new CSVReader(dialect, new StringReader("1,2,3")))
				{
					while (reader.NextRecord())
					{
						var record = reader.GetCurrentRecord();
						if (record != null && record.Length > 0)
							results.Add(record);
					}
				}
			}
		}

		[Test]
		[ExpectedException(typeof(ReaderException), ExpectedMessage = "Can't read from file: '__no_file.txt', file not exists!")]
		public void Reader_NotExistingFile_ThrowException()
		{
			IList<IList<object>> results = new List<IList<object>>();

			using (var dialect = new Dialect())
			{
				using (var reader = new CSVReader(dialect, "__no_file.txt", "utf-8"))
				{
					while (reader.NextRecord())
					{
						var record = reader.GetCurrentRecord();
						if (record != null && record.Length > 0)
							results.Add(record);
					}
				}
			}
		}

		[Test]
		public void Reader_EmptyStream_EmptyList()
		{
			using (var dialect = new Dialect())
			{
				ReadTest("", new List<IList<object>>(), dialect);
			}
		}

		[Test]
		[ExpectedException(typeof(BadFormatException), ExpectedMessage = "Bad format: ',' expected after '\"'")]
		public void Reader_QuoteInFiledDoubleQuoteTrueStrictTrue_Null()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, true, false))
			{
				ReadTest("\"ab\"c", null, dialect);
			}
		}

		[Test]
		public void Reader_QuoteInFiledNoDoubleQuote_Ok()
		{
			using (var dialect = new Dialect(false, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("\"ab\"c", new List<IList<object>> { new List<object> { "abc" } }, dialect);
			}
		}

		[Test]
		[ExpectedException(typeof(BadFormatException), ExpectedMessage = "Line contains NULL byte!")]
		public void Reader_NullByte_ThrowException()
		{
			using (var dialect = new Dialect())
			{
				ReadTest("ab\0c", null, dialect);
			}
		}

		[Test]
		[ExpectedException(typeof(BadFormatException), ExpectedMessage = "Line contains NULL byte!")]
		public void Reader_NullByteStrict_ThrowException()
		{
			using (var dialect = new Dialect(false, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, true, false))
			{
				ReadTest("ab\0c", null, dialect);
			}
		}

		[Test]
		public void Reader_EOL_Ok()
		{
			using (var dialect = new Dialect())
			{
				ReadTest("a,b", new List<IList<object>> { new List<object> { "a", "b" } }, dialect);
				ReadTest("c,d\n", new List<IList<object>> { new List<object> { "c", "d" } }, dialect);
				ReadTest("e,f\r", new List<IList<object>> { new List<object> { "e", "f" } }, dialect);
				ReadTest("h,g\r\n", new List<IList<object>> { new List<object> { "h", "g" } }, dialect);

				ReadTest(
					"a1,b1\nc1,d1",
					new List<IList<object>>
					{ 
						new List<object> { "a1", "b1" }, 
						new List<object> {"c1", "d1"} 
					},
					dialect);

				ReadTest(
					"a1,b1\rc1,d1",
					new List<IList<object>>
					{ 
						new List<object> { "a1", "b1" }, 
						new List<object> {"c1", "d1"} 
					},
					dialect);
			}
		}

		[Test]
		public void Reader_SkipInitialSpaceTrue_Ok()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest(" a, b, c", new List<IList<object>> { new List<object> { "a", "b", "c" } }, dialect);
			}
		}

		[Test]
		public void Reader_SkipInitialSpaceFalse_Ok()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest(" a, b, c", new List<IList<object>> { new List<object> { " a", " b", " c" } }, dialect);
			}
		}

		[Test]
		public void Reader_Escape_Ok()
		{
			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("a,\\b,c", new List<IList<object>> { new List<object> { "a", "b", "c" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("a,b\\,c", new List<IList<object>> { new List<object> { "a", "b,c" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("a,\"b\\,c\"", new List<IList<object>> { new List<object> { "a", "b,c" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("a,\"b,\\c\"", new List<IList<object>> { new List<object> { "a", "b,c" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("a,\"b,c\\\"\"", new List<IList<object>> { new List<object> { "a", "b,c\"" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("a,\"b,c\"\\", new List<IList<object>> { new List<object> { "a", "b,c\\" } }, dialect);
			}
		}

		[Test]
		public void Reader_Quoting_Ok()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("1,\",3,\",5", new List<IList<object>> { new List<object> { "1", ",3,", "5" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '\0', '\\', false, "\r\n", QuoteStyle.QuoteNone, false, false))
			{
				ReadTest("1,\",3,\",5", new List<IList<object>> { new List<object> { "1", "\"", "3", "\"", "5" } },
					dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteNone, false, false))
			{
				ReadTest("1,\",3,\",5", new List<IList<object>> { new List<object> { "1", "\"", "3", "\"", "5" } },
					dialect);
			}
		}

		[Test]
		public void Reader_Multiline_Ok()
		{
			using (var dialect = new Dialect(true, ';', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("1;3;5\r\n6;7;8", new List<IList<object>> 
				{ 
					new List<object> { "1", "3", "5" },
					new List<object> { "6", "7", "8" }, 
				}, dialect);
			}
		}
	}
}

