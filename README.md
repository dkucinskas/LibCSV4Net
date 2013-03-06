# LibCSV4Net #

Library for reading and writing tabular (CSV) files.

## NuGet package ##

[NuGet package](http://nuget.org/packages/LibCSV4Net/)

## Goal ##

The goal is to have library, which does not depend on third party components is fast and simple to use.

## History ##

This library started its life as port of Python CSV module. At the time I was searching for .Net library which could handle third party legacy CSV format. I didn't succeeded, so I created this library. It was tested with some huge data imports/exports.  

## API ##

 * [CSVReader class](#csvreader-class) is responsible for reading and parsing tabular data. ([CSVReader example](#csvreader-example))
 * [CSVWriter class](#csvwriter-class) is responsible for writing tabular data to stream. ([CSVWriter example](#csvwriter-example))
 * [CSVAdapter class](#csvadapter-class) is advanced csv reader/writer that supports read/write of all records and transformations. ([CSVAdapter example](#csvadapter-example))
 * [Dialect class](#dialect-class) defines rules for CSV parsing and generation options.
 
### CSVReader class ###

CSVReader class is responsible for reading and parsing tabular data. Parsing is controlled by set of rules defined in Dialect. Exposes the following operations:

  * Next() - reads and parses next record (returns true on success)
  * Current - return current record as array of strings
  * Headers - return headers as array of strings

### CSVWriter class ###

CSVWriter class is responsible for writing tabular data to stream.

### CSVAdapter class ###

CSVAdapter class is advanced csv reader/writer that supports read/write of all records and transformations.

### Dialect class ###

Dialect class defines rules for CSV parsing and generation options.

## Example ##

### CSVAdapter example ###

```c#

// Define dialect
public class ExcelDialect : Dialect
{
    public ExcelDialect()
        : base(true, ';', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, true)
    {
    }
}

// Define transformer from your custom type to tabular data.
public class ExportTransformer : IDataTransformer
{
    public object TransformTuple(object[] tuple, string[] aliases)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable TransformResult(IEnumerable result)
    {
        throw new System.NotImplementedException();
    }

    public object[] TransformRow(object tuple)
    {
        var data = tuple as CustomDto;

        if (data == null)
        {
            throw new Exception();
        }

        var row = new string[4];
        row[0] = data.Tile;
        row[1] = data.CreatedOn.ToString(CultureInfo.InvariantCulture);
        row[2] = data.Total.ToString(CultureInfo.InvariantCulture);
        row[3] = data.Description;

        return row;
    }
}

// Now we will write all data to csv file.
var data = new CustomDto[]
{
	new CustomDto { Title = "Title1", CreatedOn = DateTime.Now, Total = 1000.00, Description = "Description1" },
	new CustomDto { Title = "Title2", CreatedOn = DateTime.Now, Total = 2000.00, Description = "Description2" },
}

using (var writer = new StreamWriter(@"C:\test.csv"))
{
    using (var adapter = new CSVAdapter(new ExcelDialect(), writer, new string[]
                                                                        {
                                                                            "Tile", 
																			"CreatedOn",
                                                                            "Total",
                                                                            "Description"
                                                                        }))
    {
        adapter.WriteAll(data, new ExportTransformer());
    }
}
```
### CSVReader example ###

```c#
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
			
			var dialect = new TestDialect { 
				DoubleQuote = true,
				Delimiter = ';',
				Quote = '\'',
				Escape = '\\',
				SkipInitialSpace = true,
				LineTerminator = "\r\n",
				Quoting = QuoteStyle.QuoteNone,
				Strict = true,
				HasHeader = false
			
			using (CSVReader reader = new CSVReader(dialect, new StringReader(input))) 
            {
				while (reader.Next()) 
                {	
					string[] record = reader.Current;

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
### CSVWriter example ###

```c#
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
			var dialect = new TestDialect { 
				DoubleQuote = true,
				Delimiter = ';',
				Quote = '\'',
				Escape = '\\',
				SkipInitialSpace = true,
				LineTerminator = "\r\n",
				Quoting = QuoteStyle.QuoteNone,
				Strict = true,
				HasHeader = false

            object[] data = new object[]
            {
			    new object[] { 123, 123.45, 10M, "This is string", new DateTime(2010, 9, 3, 0, 0, 0), null },
                new object[] { 456, 456.78, 11M, "This is string too", new DateTime(2012, 04, 04, 0, 0, 0), null }
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

## Changes ##

 * 0.8.0.1648
  * Fixed issue #6: Dialect doesn't support object initializer.
  * Fixed issue #5: If property 'HasHeader' is set to 'true', then CSVReader should not return header's row on method's 'GetCurrentRecord' invocation.
  * Cleaned code
  * Updated documentatio
 * 0.7.2.1401
  * CSVReader method NextRecord renamed to Next.
  * CSVReader method GetCurrentRecord refactored to property Current.
  * Updated documentation to reflect this change.
 * 0.6.8.1105
  * Added NuGet package
 * 0.6.7.0838
  * Code cleanup
  * Headers must be specified explicitly in CSVDialect
  * Updated examples in README.md
 * 0.5
  * Improved CSVAdapter
  * Transformer supports transforming result for writing
 * 0.4
  * Added CSVWriter
 * 0.3
  * Changed IResultTransformer interface
   * IList TransformList(IList result) -> IEnumerable TransformResult(IEnumerable result);
 * 0.2
  * Redesigned CSVReader interface
   * Dialect must be set in constructor (removed property)
   * Added header support to Dialect
   * Added GetCurrentRecord
   * Record is returned as string array
  * Added IResultTransformer
  * Added Adapter class

## License ##

This is free software, and you are welcome to redistribute it under certain conditions; see LICENSE.txt for details.

## Author contact ##

Darius Kucinskas d.kucinskas@gmail.com, http://blog-of-darius.blogspot.com/

## Collaborators ##

ssharunas ssharunas@yahoo.co.uk


