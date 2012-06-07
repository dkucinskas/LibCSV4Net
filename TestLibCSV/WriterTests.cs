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
		[Test]
		public void Writer_WriteRow_Ok()
		{
			var oldCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			var row = new object[]
			{
				123, 123.45, 10M, "This is string", new DateTime(2010, 9, 3, 0, 0, 0),
				null, 1
			};

			string results;
			using (var stringWriter = new StringWriter())
			{
				using (var writer = new CSVWriter(
					new Dialect(true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QuoteMinimal, false, false),
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

