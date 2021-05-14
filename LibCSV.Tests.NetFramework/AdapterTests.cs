using LibCSV.Dialects;
using LibCSV.Exceptions;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using static NUnit.Framework.Assert;

namespace LibCSV.Tests
{
    [TestFixture]
	public class AdapterTests
	{
		[Test]
		public void ReadAll_ExistingFileName_ReturnsRecords()
		{
            var filename = Path.Combine(TestContext.CurrentContext.TestDirectory, "test.csv");

			var transformer = new NullTransformerForAdapterTesting
            (
                new [] { "Header#1", "Header#2", "Header#3" },
                new []
				{
					new [] {"1", "2", "3"},
					new [] {"4", "5", "6"}
				}
            );

            using (var dialect = new Dialect
            {
                DoubleQuote = true,
                Delimiter = ';',
                Quote = '"',
                Escape = '\\',
                SkipInitialSpace = true,
                LineTerminator = "\n\r",
                Quoting = QuoteStyle.QuoteNone,
                Strict = true,
                HasHeader = true
            })
            {
                using (var adapter = new CSVAdapter(dialect, filename, "utf-8"))
                {
                    adapter.ReadAll(transformer);
                }
            }	
		}

        [Test]
        public async Task ReadAllAsync_ExistingFileName_ReturnsRecords()
        {
            var filename = Path.Combine(TestContext.CurrentContext.TestDirectory, "test.csv");

            var transformer = new NullTransformerForAdapterTesting
            (
                new[] { "Header#1", "Header#2", "Header#3" },
                new[]
                {
                    new [] {"1", "2", "3"},
                    new [] {"4", "5", "6"}
                }
            );

            using (var dialect = new Dialect
            {
                DoubleQuote = true,
                Delimiter = ';',
                Quote = '"',
                Escape = '\\',
                SkipInitialSpace = true,
                LineTerminator = "\n\r",
                Quoting = QuoteStyle.QuoteNone,
                Strict = true,
                HasHeader = true
            })
            {
                using (var adapter = new CSVAdapter(dialect, filename, "utf-8"))
                {
                    await adapter.ReadAllAsync(transformer);
                }
            }
        }

        [Test]
		public void ReadAll_ExistingStream_ReturnsRecords()
		{
			const string input = "Header#1;Header#2;Header#3\r\n1;2;3\r\n4;5;6";
			
			var transformer = new NullTransformerForAdapterTesting
            (
                new [] { "Header#1", "Header#2", "Header#3" },
				new []
				{
					new[] {"1", "2", "3"},
					new[] {"4", "5", "6"}
				}
            );
			
			using (var dialect = new Dialect
            {
                DoubleQuote = true,
                Delimiter = ';',
                Quote = '"',
                Escape = '\\',
                SkipInitialSpace = true,
                LineTerminator = "\n\r",
                Quoting = QuoteStyle.QuoteNone,
                Strict = true,
                HasHeader = true
            })
			{
                using (var adapter = new CSVAdapter(dialect, new StringReader(input)))
                {
                    adapter.ReadAll(transformer);
                }
			}
		}

        [Test]
        public async Task ReadAllAsync_ExistingStream_ReturnsRecords()
        {
            const string input = "Header#1;Header#2;Header#3\r\n1;2;3\r\n4;5;6";

            var transformer = new NullTransformerForAdapterTesting
            (
                new[] { "Header#1", "Header#2", "Header#3" },
                new[]
                {
                    new[] {"1", "2", "3"},
                    new[] {"4", "5", "6"}
                }
            );

            using (var dialect = new Dialect
            {
                DoubleQuote = true,
                Delimiter = ';',
                Quote = '"',
                Escape = '\\',
                SkipInitialSpace = true,
                LineTerminator = "\n\r",
                Quoting = QuoteStyle.QuoteNone,
                Strict = true,
                HasHeader = true
            })
            {
                using (var adapter = new CSVAdapter(dialect, new StringReader(input)))
                {
                    await adapter.ReadAllAsync(transformer);
                }
            }
        }

        [Test]
		public void ReadAll_WithoutHeaders_ReturnRecords()
		{
			const string input = "1;2;3\r\n4;5;6";

			var transformer = new NullTransformerForAdapterTesting
            (
                expectedAliases: null,
                expectedResults: new[] 
				{
					new[] {"1", "2", "3"},
					new[] {"4", "5", "6"}
				}
            );
			
			using (var dialect = new Dialect
            {
                DoubleQuote = true,
                Delimiter = ';',
                Quote = '"',
                Escape = '\\',
                SkipInitialSpace = true,
                LineTerminator = "\n\r",
                Quoting = QuoteStyle.QuoteNone,
                Strict = true,
                HasHeader = false
            })
			{
				using (var adapter = new CSVAdapter(dialect, new StringReader(input)))
				{
					adapter.ReadAll(transformer);
				}
			}
		}

        [Test]
        public async Task ReadAllAsync_WithoutHeaders_ReturnRecords()
        {
            const string input = "1;2;3\r\n4;5;6";

            var transformer = new NullTransformerForAdapterTesting
            (
                expectedAliases: null,
                expectedResults: new[]
                {
                    new[] {"1", "2", "3"},
                    new[] {"4", "5", "6"}
                }
            );

            using (var dialect = new Dialect
            {
                DoubleQuote = true,
                Delimiter = ';',
                Quote = '"',
                Escape = '\\',
                SkipInitialSpace = true,
                LineTerminator = "\n\r",
                Quoting = QuoteStyle.QuoteNone,
                Strict = true,
                HasHeader = false
            })
            {
                using (var adapter = new CSVAdapter(dialect, new StringReader(input)))
                {
                    await adapter.ReadAllAsync(transformer);
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

			var transformer = new NullTransformerForAdapterTesting(null, data);
			
			using (var dialect = new Dialect
            {
                DoubleQuote = true,
                Delimiter = ';',
                Quote = '"',
                Escape = '\\',
                SkipInitialSpace = true,
                LineTerminator = "\n\r",
                Quoting = QuoteStyle.QuoteNone,
                Strict = true,
                HasHeader = false
            })
			{
				using (var adapter = new CSVAdapter(dialect, new StringWriter()))
				{
					adapter.WriteAll(data, transformer);
				}
			}
		}

        [Test]
        public async Task WriteAllAsync_WithoutHeaders_WroteRecords()
        {
            var data = new[]
            {
                new[] {"1", "2", "3"},
                new[] {"4", "5", "6"}
            };

            var transformer = new NullTransformerForAdapterTesting(null, data);

            using (var dialect = new Dialect
            {
                DoubleQuote = true,
                Delimiter = ';',
                Quote = '"',
                Escape = '\\',
                SkipInitialSpace = true,
                LineTerminator = "\n\r",
                Quoting = QuoteStyle.QuoteNone,
                Strict = true,
                HasHeader = false
            })
            {
                using (var adapter = new CSVAdapter(dialect, new StringWriter()))
                {
                    await adapter.WriteAllAsync(data, transformer);
                }
            }
        }

        [Test]
		public void WriteAll_WithHeader_WroteRecords()
		{
			var headers = new [] { "Header#1", "Header#2", "Header#3" };

			var data = new [] 
			{
				new [] {"1", "2", "3"},
				new [] {"4", "5", "6"}
			};

			var transformer = new NullTransformerForAdapterTesting(headers, data);
			
			using (var dialect = new Dialect
            {
                DoubleQuote = true,
                Delimiter = ';',
                Quote = '"',
                Escape = '\\',
                SkipInitialSpace = true,
                LineTerminator = "\n\r",
                Quoting = QuoteStyle.QuoteNone,
                Strict = true,
                HasHeader = true
            })
			{
				using (var adapter = new CSVAdapter(dialect, new StringWriter(), headers))
				{
					adapter.WriteAll(data, transformer);
				}
			}
		}

        [Test]
        public async Task WriteAllAsync_WithHeader_WroteRecords()
        {
            var headers = new[] { "Header#1", "Header#2", "Header#3" };

            var data = new[]
            {
                new [] {"1", "2", "3"},
                new [] {"4", "5", "6"}
            };

            var transformer = new NullTransformerForAdapterTesting(headers, data);

            using (var dialect = new Dialect
            {
                DoubleQuote = true,
                Delimiter = ';',
                Quote = '"',
                Escape = '\\',
                SkipInitialSpace = true,
                LineTerminator = "\n\r",
                Quoting = QuoteStyle.QuoteNone,
                Strict = true,
                HasHeader = true
            })
            {
                using (var adapter = new CSVAdapter(dialect, new StringWriter(), headers))
                {
                    await adapter.WriteAllAsync(data, transformer);
                }
            }
        }

        [Test]
		public void Adapter_HeaderIsNull_ThrowsException()
		{
            Throws<HeaderIsNullException>(() =>
            {
                var transformer = new NullTransformerForAdapterTesting(new string[] { }, new string[] { });

                using (var dialect = new Dialect {
                    DoubleQuote = true,
                    Delimiter = ';',
                    Quote = '"',
                    Escape = '\\',
                    SkipInitialSpace = true,
                    LineTerminator = "\n\r",
                    Quoting = QuoteStyle.QuoteNone,
                    Strict = true,
                    HasHeader = true
                })
                {
                    using (var adapter = new CSVAdapter(dialect, new StringWriter(), null))
                    {

                    }
                }
            });
		}

        [Test]
		public void WriteAll_DataTransformerIsNull_ThrowsException()
		{
            Throws<DataTransformerIsNullException>(() =>
            {
                var headers = new [] { "Header#1", "Header#2", "Header#3" };

                var data = new []
                {
                    new [] {"1", "2", "3"},
                    new [] {"4", "5", "6"}
                };

                using (var dialect = new Dialect
                {
                    DoubleQuote = true,
                    Delimiter = ';',
                    Quote = '"',
                    Escape = '\\',
                    SkipInitialSpace = true,
                    LineTerminator = "\n\r",
                    Quoting = QuoteStyle.QuoteNone,
                    Strict = true,
                    HasHeader = false
                })
                {
                    using (var adapter = new CSVAdapter(dialect, new StringWriter(), headers))
                    {
                        adapter.WriteAll(data, null);
                    }
                }
            });
		}

        [Test]
        public void WriteAllAsync_DataTransformerIsNull_ThrowsException()
        {
            ThrowsAsync<DataTransformerIsNullException>(async () =>
            {
                var headers = new[] { "Header#1", "Header#2", "Header#3" };

                var data = new[]
                {
                    new [] {"1", "2", "3"},
                    new [] {"4", "5", "6"}
                };

                using (var dialect = new Dialect
                {
                    DoubleQuote = true,
                    Delimiter = ';',
                    Quote = '"',
                    Escape = '\\',
                    SkipInitialSpace = true,
                    LineTerminator = "\n\r",
                    Quoting = QuoteStyle.QuoteNone,
                    Strict = true,
                    HasHeader = false
                })
                {
                    using (var adapter = new CSVAdapter(dialect, new StringWriter(), headers))
                    {
                        await adapter.WriteAllAsync(data, null);
                    }
                }
            });
        }

        [Test]
		public void WriteAll_NotEqualCellCountInRows_ThrowsException()
		{
            Throws<NotEqualCellCountInRowsException>(() =>
            {
                var headers = new [] { "Header#1", "Header#2", "Header#3" };

                var data = new []
                {
                    new [] {"1", "2", "3"},
                    new [] {"4", "5"}
                };

                var transformer = new NullTransformerForAdapterTesting(headers, data);

                using (var dialect = new Dialect {
                    DoubleQuote = true, 
                    Delimiter = ';',
                    Quote = '"',
                    Escape = '\\',
                    SkipInitialSpace = true,
                    LineTerminator = "\n\r",
                    Quoting = QuoteStyle.QuoteNone,
                    Strict = true,
                    HasHeader = false
                })
                {
                    using (var adapter = new CSVAdapter(dialect, new StringWriter(), null))
                    {
                        adapter.WriteAll(data, transformer);
                    }
                }
            });
		}

        [Test]
        public void WriteAllAsync_NotEqualCellCountInRows_ThrowsException()
        {
            ThrowsAsync<NotEqualCellCountInRowsException>(async () =>
            {
                var headers = new[] { "Header#1", "Header#2", "Header#3" };

                var data = new[]
                {
                    new [] {"1", "2", "3"},
                    new [] {"4", "5"}
                };

                var transformer = new NullTransformerForAdapterTesting(headers, data);

                using (var dialect = new Dialect
                {
                    DoubleQuote = true,
                    Delimiter = ';',
                    Quote = '"',
                    Escape = '\\',
                    SkipInitialSpace = true,
                    LineTerminator = "\n\r",
                    Quoting = QuoteStyle.QuoteNone,
                    Strict = true,
                    HasHeader = false
                })
                {
                    using (var adapter = new CSVAdapter(dialect, new StringWriter(), null))
                    {
                        await adapter.WriteAllAsync(data, transformer);
                    }
                }
            });
        }
    }
}

