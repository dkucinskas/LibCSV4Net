using LibCSV;
using LibCSV.Dialects;
using NUnit.Framework;

namespace TestLibCSV
{
	class TestDialect : Dialect
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
				Assert.AreEqual(dialect.DoubleQuote, true);
				Assert.AreEqual(dialect.Delimiter, ';');
				Assert.AreEqual(dialect.Quote, '\'');
				Assert.AreEqual(dialect.Escape, '\\');
				Assert.AreEqual(dialect.SkipInitialSpace, true);
				Assert.AreEqual(dialect.LineTerminator, "\r\n");
				Assert.AreEqual(dialect.Quoting, QuoteStyle.QuoteNone);
				Assert.AreEqual(dialect.Strict, true);
				Assert.AreEqual(dialect.HasHeader, false);
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
				Assert.AreEqual(dialect.DoubleQuote, true);
				Assert.AreEqual(dialect.Delimiter, ';');
				Assert.AreEqual(dialect.Quote, '\'');
				Assert.AreEqual(dialect.Escape, '\\');
				Assert.AreEqual(dialect.SkipInitialSpace, true);
				Assert.AreEqual(dialect.LineTerminator, "\r\n");
				Assert.AreEqual(dialect.Quoting, QuoteStyle.QuoteNone);
				Assert.AreEqual(dialect.Strict, true);
				Assert.AreEqual(dialect.HasHeader, false);
			}
		}
	}
}

