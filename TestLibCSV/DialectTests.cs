using LibCSV;
using LibCSV.Dialects;
using NUnit.Framework;

namespace TestLibCSV
{
	class TestDialect : Dialect
	{
		public TestDialect()
			: base(true, ';', '\'', '\\', true, "\r\n", QuoteStyle.QUOTE_NONE, true, false)
		{
		}
	}

	[TestFixture]
	public class DialectTests
	{
		public DialectTests()
		{
		}

		[Test]
		public void Dialect_CustomDialectEqual_Ok()
		{
			using (TestDialect dialect = new TestDialect())
			{
				Assert.AreEqual(dialect.DoubleQuote, true);
				Assert.AreEqual(dialect.Delimiter, ';');
				Assert.AreEqual(dialect.Quote, '\'');
				Assert.AreEqual(dialect.Escape, '\\');
				Assert.AreEqual(dialect.SkipInitialSpace, true);
				Assert.AreEqual(dialect.LineTerminator, "\r\n");
				Assert.AreEqual(dialect.Quoting, QuoteStyle.QUOTE_NONE);
				Assert.AreEqual(dialect.Strict, true);
				Assert.AreEqual(dialect.HasHeader, false);
			}
		}
	}
}

