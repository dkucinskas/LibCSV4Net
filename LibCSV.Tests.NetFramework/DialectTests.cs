using LibCSV;
using LibCSV.Dialects;
using LibCSV.Exceptions;
using NUnit.Framework;

using static NUnit.Framework.Assert;

namespace LibCSV.Tests
{
	public class TestDialect : Dialect
	{
		public TestDialect()
			: base(true, ';', '\'', '\\', true, "\r\n", QuoteStyle.QuoteNone, true, false)
		{
		}
	}

	[TestFixture]
	public class DialectTests
	{
		[Test]
		public void Dialect_CustomDialectEqual_Ok()
		{
			using (var dialect = new TestDialect())
			{
				AreEqual(dialect.DoubleQuote, true);
				AreEqual(dialect.Delimiter, ';');
				AreEqual(dialect.Quote, '\'');
				AreEqual(dialect.Escape, '\\');
				AreEqual(dialect.SkipInitialSpace, true);
				AreEqual(dialect.LineTerminator, "\r\n");
				AreEqual(dialect.Quoting, QuoteStyle.QuoteNone);
				AreEqual(dialect.Strict, true);
				AreEqual(dialect.HasHeader, false);
			}
		}

		[Test]
		public void Dialect_WithObjectInitializer_Ok()
		{
			using (var dialect = new TestDialect { 
				DoubleQuote = true,
				Delimiter = ';',
				Quote = '\'',
				Escape = '\\',
				SkipInitialSpace = true,
				LineTerminator = "\r\n",
				Quoting = QuoteStyle.QuoteNone,
				Strict = true,
				HasHeader = false
			})
			{
				AreEqual(dialect.DoubleQuote, true);
				AreEqual(dialect.Delimiter, ';');
				AreEqual(dialect.Quote, '\'');
				AreEqual(dialect.Escape, '\\');
				AreEqual(dialect.SkipInitialSpace, true);
				AreEqual(dialect.LineTerminator, "\r\n");
				AreEqual(dialect.Quoting, QuoteStyle.QuoteNone);
				AreEqual(dialect.Strict, true);
				AreEqual(dialect.HasHeader, false);
			}
		}
		
		[Test]
		public void Dialect_ShouldThrowDialectInternalErrorExceptionThenQuotingNotSet_Ok()
		{
            Throws<DialectInternalErrorException>(() =>
            {
                using (var dialect = new TestDialect
                {
                    DoubleQuote = true,
                    Delimiter = ';',
                    Quote = '\0',
                    Escape = '\\',
                    SkipInitialSpace = true,
                    LineTerminator = "\r\n",
                    Quoting = QuoteStyle.QuoteAll,
                    Strict = true,
                    HasHeader = false
                })
                {
                    dialect.Check();
                }
            });
		}
		
		[Test]
		public void Dialect_ShouldThrowDialectInternalErrorExceptionThenLineTerminatorNotSet_Ok()
		{
            Throws<DialectInternalErrorException>(() =>
            {
                using (var dialect = new TestDialect
                {
                    DoubleQuote = true,
                    Delimiter = ';',
                    Quote = '\'',
                    Escape = '\\',
                    SkipInitialSpace = true,
                    LineTerminator = null,
                    Quoting = QuoteStyle.QuoteAll,
                    Strict = true,
                    HasHeader = false
                })
                {
                    dialect.Check();
                }
            });
		}
	}
}

