# LibCSV

Library for reading and writing tabular (CSV) files.

## Cahnges

### 0.5
 * Improved CSVAdapter
 * Transformer supports transforming result for writing

### 0.4
 * Added CSVWriter

### 0.3
 * Changed IResultTransformer interface
  * IList TransformList(IList result) -> IEnumerable TransformResult(IEnumerable result);
 
### 0.2 

 * Redesigne of CsvReader interface
  * Dialect must be set in constructor (removed property)
  * Added header support to Dialect
  * Added GetCurrentRecord
  * Recorrd is returned as string array
 * Added IResultTransformer
 * Added Adapter class

## Example

 * The following is simple example of using CSVReader

``` c#
using System;
using System.Collections.Generic;
using System.IO;
using LibCSV;
using LibCSV.Dialects;

namespace LibCSV4NetApp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string input = "Header#1;Header#2;Header#3\r\n1;2;3\r\n4;5;6\r\ntest1;234;test2";
			
			Dialect dialect = new Dialect (
				true, ';', '\'', '\\', true, "\r\n", QuoteStyle.QUOTE_NONE, true, false);
			
			using (CSVReader reader = new CSVReader(dialect, new StringReader(input))) 
            {
				while (reader.NextRecord()) 
                {	
					string[] record = reader.GetCurrentRecord ();

                    if (record != null && record.Length > 0)
                    {
                        foreach (string item in record)
                            Console.Write(item + "| ");
                        Console.WriteLine();
                    }
					
					record = null;
				}
			}
			
			Console.ReadLine ();
		}
	}
}
```

 * The following is simple example of using CSVWriter

``` c#
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibCSV;
using LibCSV.Dialects;

namespace LibCSV4NetApp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            Dialect dialect = new Dialect(
                true, ';', '\"', '\\', true, "\r\n", QuoteStyle.QUOTE_MINIMAL, false, false);

            object[] data = new object[]
            {
			    new object[] { 123, 123.45, 10M, "This is string", new DateTime(2010, 9, 3, 0, 0, 0), null },
                new object[] { 456, 456.78, 11M, "This is string too", new DateTime(2012, 04, 04, 0, 0, 0), null
                }
            };

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (TextWriter textWriter = new StreamWriter(memoryStream))
                using (CSVWriter writer = new CSVWriter(dialect, textWriter))
                {
                    foreach (object[] row in data)
                        writer.WriteRow(row);
                }

                Encoding encoding = Encoding.ASCII;
                Console.Write(encoding.GetString(memoryStream.ToArray()));
            }

			Console.ReadLine ();
		}
	}
}

```
 * The following is simple example of using CSVAdapter

## License

This is free software, and you are welcome to redistribute it under certain conditions; see LICENSE.txt for details.

## Author contact

Darius Kucinskas d.kucinskas@gmail.com, http://blog-of-darius.blogspot.com/

## Collaborators

ssharunas ssharunas@yahoo.co.uk


