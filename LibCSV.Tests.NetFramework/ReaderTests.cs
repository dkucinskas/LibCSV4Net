using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LibCSV.Dialects;
using LibCSV.Exceptions;
using NUnit.Framework;

using static NUnit.Framework.Assert;

namespace LibCSV.Tests
{
	[TestFixture]
	public class ReaderTests : ReaderBaseTests
	{
		[Test]
		public void Next_EmptyStream_ReturnsEmptyList()
		{
			using (var dialect = new Dialect())
			{
				ReadTest("", new List<IList<object>>(), dialect);
			}
		}

		[Test]
		public async Task NextAsync_EmptyStream_ReturnsEmptyList()
		{
			using (var dialect = new Dialect())
			{
				await ReadTestAsync("", new List<IList<object>>(), dialect);
			}
		}

		[Test]
		public void Next_FirstLine_HeadersArePopulated()
		{
			using (var dialect = new Dialect(false, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, true, true))
			{
				using (var reader = new CSVReader(dialect, new StringReader("a,b,c")))
				{
					reader.Open();

					reader.Next();
					var current = reader.Current;
					
					IsNotNull(reader.Headers);
					AreEqual(new[] { "a", "b", "c" }, reader.Headers);
				}
			}
		}

		[Test]
		public async Task NextAsync_FirstLine_HeadersArePopulated()
		{
			using (var dialect = new Dialect(false, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, true, true))
			{
				using (var reader = new CSVReader(dialect, new StringReader("a,b,c")))
				{
					reader.Open();

					await reader.NextAsync();
					var current = reader.Current;

					IsNotNull(reader.Headers);
					AreEqual(new[] { "a", "b", "c" }, reader.Headers);
				}
			}
		}

		[Test]
		public void Next_EndOfLine_LineIsRead()
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
				
				ReadTest("\n\r", new List<IList<object>> { }, dialect);
			}
		}

		[Test]
		public async Task NextAsync_EndOfLine_LineIsRead()
		{
			using (var dialect = new Dialect())
			{
				await ReadTestAsync("a,b", new List<IList<object>> { new List<object> { "a", "b" } }, dialect);
				await ReadTestAsync("c,d\n", new List<IList<object>> { new List<object> { "c", "d" } }, dialect);
				await ReadTestAsync("e,f\r", new List<IList<object>> { new List<object> { "e", "f" } }, dialect);
				await ReadTestAsync("h,g\r\n", new List<IList<object>> { new List<object> { "h", "g" } }, dialect);

				await ReadTestAsync(
					"a1,b1\nc1,d1",
					new List<IList<object>>
					{
						new List<object> { "a1", "b1" },
						new List<object> {"c1", "d1"}
					},
					dialect);

				await ReadTestAsync(
					"a1,b1\rc1,d1",
					new List<IList<object>>
					{
						new List<object> { "a1", "b1" },
						new List<object> {"c1", "d1"}
					},
					dialect);

				await ReadTestAsync("\n\r", new List<IList<object>> { }, dialect);
			}
		}

		[Test]
		public void Next_WithSkipInitialSpaceFlag_SkipedInitialSpaces()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest(" a, b, c", new List<IList<object>> { new List<object> { "a", "b", "c" } }, dialect);
			}
		}

		[Test]
		public async Task NextAsync_WithSkipInitialSpaceFlag_SkipedInitialSpaces()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync(" a, b, c", new List<IList<object>> { new List<object> { "a", "b", "c" } }, dialect);
			}
		}

		[Test]
		public void Next_WithoutSkipInitialSpaceFlag_InitialSpacesPresent()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest(" a, b, c", new List<IList<object>> { new List<object> { " a", " b", " c" } }, dialect);
			}
		}

		[Test]
		public async Task NextAsync_WithoutSkipInitialSpaceFlag_InitialSpacesPresent()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync(" a, b, c", new List<IList<object>> { new List<object> { " a", " b", " c" } }, dialect);
			}
		}

		[Test]
		public void Next_EscapedSequecesRead_SequencesRead()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest(" a, b, c", new List<IList<object>> { new List<object> { " a", " b", " c" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("a,\"b,c\\n\\t\\r\"\\", new List<IList<object>> { new List<object> { "a", "b,c\n\t\r\\" } }, dialect);
			}
		}

		[Test]
		public async Task NextAsync_EscapedSequecesRead_SequencesRead()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync(" a, b, c", new List<IList<object>> { new List<object> { " a", " b", " c" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync("a,\"b,c\\n\\t\\r\"\\", new List<IList<object>> { new List<object> { "a", "b,c\n\t\r\\" } }, dialect);
			}
		}

		[Test]
		public void Next_WithEscapeFlag_SymbolAreEscaped()
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

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("a,\"b,c\\n\\t\\r\"\\", new List<IList<object>> { new List<object> { "a", "b,c\n\t\r\\" } }, dialect);
			}
		}

		[Test]
		public async Task NextAsync_WithEscapeFlag_SymbolAreEscaped()
		{
			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync("a,\\b,c", new List<IList<object>> { new List<object> { "a", "b", "c" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync("a,b\\,c", new List<IList<object>> { new List<object> { "a", "b,c" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync("a,\"b\\,c\"", new List<IList<object>> { new List<object> { "a", "b,c" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync("a,\"b,\\c\"", new List<IList<object>> { new List<object> { "a", "b,c" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync("a,\"b,c\\\"\"", new List<IList<object>> { new List<object> { "a", "b,c\"" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync("a,\"b,c\"\\", new List<IList<object>> { new List<object> { "a", "b,c\\" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync("a,\"b,c\\n\\t\\r\"\\", new List<IList<object>> { new List<object> { "a", "b,c\n\t\r\\" } }, dialect);
			}
		}

		[Test]
		public void Next_WithQuoting_Quoted()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("1,\",3,\",5", new List<IList<object>> { new List<object> { "1", ",3,", "5" } }, dialect);
			}
			
			using (var dialect = new Dialect(true, ',', '\0', '\\', false, "\r\n", QuoteStyle.QuoteNone, false, false))
			{
				ReadTest("1,\",3,\",5", new List<IList<object>> { new List<object> { "1", "\"", "3", "\"", "5" } }, dialect);
			}
			
			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteNone, false, false))
			{
				ReadTest("1,\",3,\",5", new List<IList<object>> { new List<object> { "1", "\"", "3", "\"", "5" } }, dialect);
			}
			
			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteAll, false, false))
			{
				ReadTest("\"1\"\",3\",5", new List<IList<object>> { new List<object> { "1\",3", "5" } }, dialect);
			}
		}

		[Test]
		public async Task NextAsync_WithQuoting_Quoted()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync("1,\",3,\",5", new List<IList<object>> { new List<object> { "1", ",3,", "5" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '\0', '\\', false, "\r\n", QuoteStyle.QuoteNone, false, false))
			{
				await ReadTestAsync("1,\",3,\",5", new List<IList<object>> { new List<object> { "1", "\"", "3", "\"", "5" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteNone, false, false))
			{
				await ReadTestAsync("1,\",3,\",5", new List<IList<object>> { new List<object> { "1", "\"", "3", "\"", "5" } }, dialect);
			}

			using (var dialect = new Dialect(true, ',', '"', '\\', false, "\r\n", QuoteStyle.QuoteAll, false, false))
			{
				await ReadTestAsync("\"1\"\",3\",5", new List<IList<object>> { new List<object> { "1\",3", "5" } }, dialect);
			}
		}

		[Test]
		public void Next_Multiline_ReturnedMulitpleRecords()
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

		[Test]
		public async Task NextAsync_Multiline_ReturnedMulitpleRecords()
		{
			using (var dialect = new Dialect(true, ';', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync("1;3;5\r\n6;7;8", new List<IList<object>>
				{
					new List<object> { "1", "3", "5" },
					new List<object> { "6", "7", "8" },
				}, dialect);
			}
		}

		[Test]
		public void Next_WithHeaders_HeadersAreNotReturnedAsFirstRecord()
		{
			using (var dialect = new Dialect(true, ';', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, true))
			{
				ReadTest("Header#1;Header#2;Header#3\r\n1;2;3\r\n4;5;6\r\ntest1;234;test2", new List<IList<object>>
				{
					new List<object> { "1", "2", "3" },
					new List<object> { "4", "5", "6" }, 
					new List<object> { "test1", "234", "test2" }, 
				}, dialect);
			}
		}

		[Test]
		public async Task NextAsync_WithHeaders_HeadersAreNotReturnedAsFirstRecord()
		{
			using (var dialect = new Dialect(true, ';', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, true))
			{
				await ReadTestAsync("Header#1;Header#2;Header#3\r\n1;2;3\r\n4;5;6\r\ntest1;234;test2", new List<IList<object>>
				{
					new List<object> { "1", "2", "3" },
					new List<object> { "4", "5", "6" },
					new List<object> { "test1", "234", "test2" },
				}, dialect);
			}
		}

		[Test]
		public void Next_WithHeaders_HeadersAreInitialized()
		{
			string input = "Header#1;Header#2;Header#3\r\n1;2;3\r\n4;5;6\r\ntest1;234;test2";
			using (var dialect = new Dialect(true, ';', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, true))
			{
				using (var reader = new CSVReader(dialect, new StringReader(input)))
				{
					reader.Open();

					IsNotNull(reader.Headers);
				}
			}
		}

		[Test]
		public void Next_EmptyLine_NextReturnsFalse()
		{
			string input = "Header#1;Header#2;Header#3\r\n1;2;3\r\n \r\n";
			using (var dialect = new Dialect(true, ';', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, true))
			{
				using (var reader = new CSVReader(dialect, new StringReader(input)))
				{
					reader.Open();

					IsTrue(reader.Next());
					IsFalse(reader.Next());
				}
			}
		}

		[Test]
		public async Task NextAsync_EmptyLine_NextReturnsFalse()
		{
			string input = "Header#1;Header#2;Header#3\r\n1;2;3\r\n \r\n";
			using (var dialect = new Dialect(true, ';', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, true))
			{
				using (var reader = new CSVReader(dialect, new StringReader(input)))
				{
					await reader.OpenAsync();

					IsTrue(await reader.NextAsync());
					IsFalse(await reader.NextAsync());
				}
			}
		}

		[Test]
		[Ignore("TODO: should we add ignore null byte attribute to dialect?")]
		public void Reader_ShouldSkipNullChar_Skiped()
		{
			using (var dialect = new Dialect(true, ';', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("1\02\r\n", new List<IList<object>>
				{
					new List<object> { "12" },
				}, dialect);
			}
		}
		
		[Test]
		public void Reader_NullStream_ThrowsException()
		{
            Throws<TextReaderIsNullException>(() =>
            {
                using (var dialect = new Dialect())
                {
                    using (var reader = new CSVReader(dialect, null))
                    {
                    }
                }
            });
		}
		
		[Test]
		public void Reader_NullDialect_ThrowsException()
		{
            Throws<DialectIsNullException>(() =>
            {
                using (var reader = new CSVReader(null, new StringReader("1,2,3")))
                {
                }
            });
		}
		
		[Test]
		public void Reader_DialectInternalError_ThrowsException()
		{
            Throws<DialectInternalErrorException>(() =>
            {
                using (var dialect = new Dialect(true, '\0', '"', '\\', true, "\r\n", QuoteStyle.QuoteMinimal, true, false))
                {
                }
            });
		}
		
		[Test]
		[TestCase(null)]
		[TestCase("")]
		[TestCase("  ")]
		public void Reader_FileNameIsNullOrEmpty_ThrowsException(string fileName)
		{
            Throws<FileNameIsNullOrEmptyException>(() =>
            {
                using (var dialect = new Dialect(true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
                {
                    using (var csvReader = new CSVReader(dialect, fileName, "UTF-8"))
                    {
                    }
                }
            });
		}
		
		[Test]
		public void Reader_CannotReadFromFile_ThrowsException()
		{
            Throws<CannotReadFromFileException>(() =>
            {
                using (var dialect = new Dialect())
                {
                    using (var reader = new CSVReader(dialect, "VeryLongNonExistingFileNameForTesting", "utf-8"))
                    {
                    }
                }
            });
		}
		
		[Test]
		public void Reader_FileIsLocked__ThrowsException()
		{
            Throws<CannotReadFromFileException>(() =>
            {
                using (var writer = new StreamWriter("test_write_file_locked.csv", false, Encoding.GetEncoding("utf-8")))
                {
                    using (var dialect = new Dialect(true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
                    {
                        using (var csvReader = new CSVReader(dialect, "test_write_file_locked.csv", "UTF-8"))
                        {
                        }
                    }
                }
            });
		}

		[Test]
		public void Reader_DialectIsNull_ThrowsException()
		{
            Throws<DialectIsNullException>(() => 
            {
                using (var reader = new CSVReader(null, "VeryLongNonExistingFileNameForTesting", "UTF-8"))
                {
                }
            });
		}

		[Test]
		public void Reader_QuoteInFieldWithdDoubleQuoteAndStrictFlags_ReturnsNull()
		{
            Throws<BadFormatException>(() =>
            {
                using (var dialect = new Dialect(true, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, true, false))
                {
                    ReadTest("\"ab\"c", null, dialect);
                }
            }, "Bad format: ',' expected after '\"'");
		}

		[Test]
		public void ReaderAsync_QuoteInFieldWithdDoubleQuoteAndStrictFlags_ReturnsNull()
		{
			ThrowsAsync<BadFormatException>(async() =>
			{
				using (var dialect = new Dialect(true, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, true, false))
				{
					await ReadTestAsync("\"ab\"c", null, dialect);
				}
			}, "Bad format: ',' expected after '\"'");
		}

		[Test]
		public void Reader_MultilineInQuotes_RetunsRecord()
		{
			using (var dialect = new Dialect(true, ',', '"','\0', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("\"This is\r\nA multi-line\r\ninput\"", new List<IList<object>>
				{
					new List<object> { "This is\r\nA multi-line\r\ninput" }
				}, dialect);
			}
		}

		[Test]
		public async Task ReaderAsync_MultilineInQuotes_RetunsRecord()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync("\"This is\r\nA multi-line\r\ninput\"", new List<IList<object>>
				{
					new List<object> { "This is\r\nA multi-line\r\ninput" }
				}, dialect);
			}
		}

		[Test]
		public void Reader_SkipsEmptyLines_ReturnsRecords()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("\"abc\",0,1\r\n,,\r\n,,\r\n\"def\",2,3", new List<IList<object>>
				{
					new List<object> { "abc", "0", "1" },
					new List<object> { "def", "2", "3" }
				}, dialect);
			}
		}

		[Test]
		public async Task ReaderAsync_SkipsEmptyLines_ReturnsRecords()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync("\"abc\",0,1\r\n,,\r\n,,\r\n\"def\",2,3", new List<IList<object>>
				{
					new List<object> { "abc", "0", "1" },
					new List<object> { "def", "2", "3" }
				}, dialect);
			}
		}

		[Test]
		public async Task Reader_QuoteInFieldWithoutDoubleQuote_ReturnsRecord()
		{
			using (var dialect = new Dialect(false, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				await ReadTestAsync("\"ab\"c", new List<IList<object>> { new List<object> { "abc" } }, dialect);
			}
		}

		[Test]
		public void Reader_NullByte_ThrowsException()
		{
			Throws<BadFormatException>(() =>
			{
				using (var dialect = new Dialect())
				{
					ReadTest("ab\0c", null, dialect);
				}
			}, "Line contains NULL byte!");

		}

		[Test]
		public void ReaderAsync_NullByte_ThrowsException()
		{
            ThrowsAsync<BadFormatException>(async () =>
            {
                using (var dialect = new Dialect())
                {
                    await ReadTestAsync("ab\0c", null, dialect);
                }
            }, "Line contains NULL byte!");

		}
		
		[Test]
		public void Reader_NullByteAndStrictDialect_ThrowException()
		{
            Throws<BadFormatException>(() =>
            {
                using (var dialect = new Dialect(false, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, true, false))
                {
                    ReadTest("ab\0c", null, dialect);
                }
            }, "Line contains NULL byte!");
		}

		[Test]
		public void ReaderAsync_NullByteAndStrictDialect_ThrowException()
		{
			ThrowsAsync<BadFormatException>(async () =>
			{
				using (var dialect = new Dialect(false, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, true, false))
				{
					await ReadTestAsync("ab\0c", null, dialect);
				}
			}, "Line contains NULL byte!");
		}
	}
}

