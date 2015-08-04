using System.Collections.Generic;
using System.IO;
using System.Text;

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
		public void Next_EmptyStream_ReturnsEmptyList()
		{
			using (var dialect = new Dialect())
			{
				ReadTest("", new List<IList<object>>(), dialect);
			}
		}
		
		[Test]
		public void Next_FirstLine_HeadersArePopulated()
		{
			using (var dialect = new Dialect(false, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, true, true))
			{
				using (var reader = new CSVReader(dialect, new StringReader("a,b,c")))
				{
					reader.Next();
					var current = reader.Current;
					
					Assert.IsNotNull(reader.Headers);
					Assert.AreEqual(new[] { "a", "b", "c" }, reader.Headers);
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
		public void Next_WithSkipInitialSpaceFlag_SkipedInitialSpaces()
		{
			using (var dialect = new Dialect(true, ',', '"', '\0', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest(" a, b, c", new List<IList<object>> { new List<object> { "a", "b", "c" } }, dialect);
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
		public void Next_WithHeaders_HeadersAreInitialized()
		{
			string input = "Header#1;Header#2;Header#3\r\n1;2;3\r\n4;5;6\r\ntest1;234;test2";
			using (var dialect = new Dialect(true, ';', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, true))
			{
				using (var reader = new CSVReader(dialect, new StringReader(input)))
				{
					Assert.IsNotNull(reader.Headers);
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
					Assert.IsTrue(reader.Next());
					Assert.IsFalse(reader.Next());
				}
			}
		}
		
		//TODO: should we add ignore null byte attribute to dialect?
		[Test]
		[Ignore]
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
		[ExpectedException(typeof(TextReaderIsNullException))]
		public void Reader_NullStream_ThrowsException()
		{
			using (var dialect = new Dialect())
			{
				using (var reader = new CSVReader(dialect, null))
				{
				}
			}
		}
		
		[Test]
		[ExpectedException(typeof(DialectIsNullException))]
		public void Reader_NullDialect_ThrowsException()
		{
			using (var reader = new CSVReader(null, new StringReader("1,2,3")))
			{
			}
		}
		
		[Test]
		[ExpectedException(typeof(DialectInternalErrorException))]
		public void Reader_DialectInternalError_ThrowsException()
		{
			using (var dialect = new Dialect(true, '\0', '"', '\\', true, "\r\n", QuoteStyle.QuoteMinimal, true, false))
			{
			}
		}
		
		[Test]
		[ExpectedException(typeof(FileNameIsNullOrEmptyException))]
		[TestCase(null)]
		[TestCase("")]
		[TestCase("  ")]
		public void Reader_FileNameIsNullOrEmpty_ThrowsException(string fileName)
		{
			using (var dialect = new Dialect(true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				using (var csvReader = new CSVReader(dialect, fileName, "UTF-8")) 
				{
				}
			}
		}
		
		[Test]
		[ExpectedException(typeof(CannotReadFromFileException))]
		public void Reader_CannotReadFromFile_ThrowsException()
		{
			using (var dialect = new Dialect())
			{
				using (var reader = new CSVReader(dialect, "VeryLongNonExistingFileNameForTesting", "utf-8")) 
				{
				}
			}
		}
		
		[Test]
		[ExpectedException(typeof(CannotReadFromFileException))]
		public void Reader_FileIsLocked__ThrowsException()
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
		}

        [Test]
        [ExpectedException(typeof(DialectIsNullException))]
        public void Reader_DialectIsNull_ThrowsException()
        {
            using (var reader = new CSVReader(null, "VeryLongNonExistingFileNameForTesting", "UTF-8"))
            {
            }
        }

        [Test]
        [ExpectedException(typeof(BadFormatException), ExpectedMessage = "Bad format: ',' expected after '\"'")]
        public void Reader_QuoteInFieldWithdDoubleQuoteAndStrictFlags_ReturnsNull()
        {
            using (var dialect = new Dialect(true, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, true, false))
            {
                ReadTest("\"ab\"c", null, dialect);
            }
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
		public void Reader_QuoteInFieldWithoutDoubleQuote_ReturnsRecord()
		{
			using (var dialect = new Dialect(false, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, false))
			{
				ReadTest("\"ab\"c", new List<IList<object>> { new List<object> { "abc" } }, dialect);
			}
		}
		
		[Test]
		[ExpectedException(typeof(BadFormatException), ExpectedMessage = "Line contains NULL byte!")]
		public void Reader_NullByte_ThrowsException()
		{
			using (var dialect = new Dialect())
			{
				ReadTest("ab\0c", null, dialect);
			}
		}
		
		[Test]
		[ExpectedException(typeof(BadFormatException), ExpectedMessage = "Line contains NULL byte!")]
		public void Reader_NullByteAndStrictDialect_ThrowException()
		{
			using (var dialect = new Dialect(false, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, true, false))
			{
				ReadTest("ab\0c", null, dialect);
			}
		}
	}
}

