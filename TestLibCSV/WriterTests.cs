using System;
using System.Globalization;
using System.IO;
using System.Threading;
using LibCSV;
using LibCSV.Dialects;
using NUnit.Framework;

namespace TestLibCSV
{
	[TestFixture]
	public class WriterTests
	{
		public WriterTests()
		{
		}

		[Test]
		public void Writer_WriteRow_Ok()
		{
			CultureInfo oldCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			object[] row = new object[]
			{
				123, 123.45, 10M, "This is string", new DateTime(2010, 9, 3, 0, 0, 0),
				null, 1
			};

			string results = null;
			using (StringWriter stringWriter = new StringWriter())
			{
				using (CSVWriter writer = new CSVWriter(
					new Dialect(true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QUOTE_MINIMAL, false, false),
					stringWriter))
				{
					writer.WriteRow(row);
				}

				results = stringWriter.ToString();
			}

			Assert.AreEqual("123;123.45;10;\"This is string\";09/03/2010 00:00:00;;1\r\n", results);

			Thread.CurrentThread.CurrentCulture = oldCulture;
		}
	}
}

