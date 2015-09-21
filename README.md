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

// Define your custom Data transfer object
public class Dto 
{
	public string Title { get; set; }
	public DateTime CreatedOn { get; set; }
	public decimal Total { get; set; } 
	public string Description { get; set; }
}

// Define dialect
var dialect = new Dialect 
{
	DoubleQuote = true,
	Delimiter = ';',
	Quote = '"',
	Escape = '\0',
	SkipInitialSpace = false,
	LineTerminator = "\r\n",
	Quoting = QuoteStyle.QuoteMinimal,
	Strict = false,
	HasHeader = false
};

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
		var data = tuple as Dto;
		if (data == null)
		{
			throw new CsvException("Row is empty!");
		}
			
		var row = new string[4];
		row[0] = data.Title;
		row[1] = data.CreatedOn.ToString(CultureInfo.InvariantCulture);
		row[2] = data.Total.ToString(CultureInfo.InvariantCulture);
		row[3] = data.Description;
		return row;
	}
}

// Now we will write all data to csv file.
var data = new[]
{
	new Dto { Title = "Title1", CreatedOn = DateTime.Now, Total = 1000.00M, Description = "Description1" },
	new Dto { Title = "Title2", CreatedOn = DateTime.Now, Total = 2000.00M, Description = "Description2" },
};

var headers = new[] { "Tile", "CreatedOn", "Total", "Description" };
using (var writer = new StreamWriter(@"C:\test.csv"))
{
	using (var adapter = new CSVAdapter(dialect, writer, headers))
	{
		adapter.WriteAll(data, new ExportTransformer());
	}
}
```
### CSVReader example ###

```c#
string input = "Header#1;Header#2;Header#3\r\n1;2;3\r\n4;5;6";

var dialect = new Dialect
{
	DoubleQuote = true,
	Delimiter = ';',
	Quote = '\'',
	Escape = '\\',
	SkipInitialSpace = true,
	LineTerminator = "\r\n",
	Quoting = QuoteStyle.QuoteNone,
	Strict = true,
	HasHeader = true
};

using (var reader = new CSVReader(dialect, new StringReader(input))) 
{
	foreach (var item in reader.Headers)
	{
		Console.Write(item + "| ");
	}
	Console.WriteLine();
		
	while (reader.Next())
	{
		var record = reader.Current;
		if (record != null && record.Length > 0)
		{
			foreach (var item in record)
			{
				Console.Write(item + "| ");
			}
			Console.WriteLine();
		}
	}
}
```
### CSVWriter example ###

```c#
var dialect = new Dialect 
{
	DoubleQuote = true,
	Delimiter = ';',
	Quote = '\'',
	Escape = '\\',
	SkipInitialSpace = true,
	LineTerminator = "\r\n",
	Quoting = QuoteStyle.QuoteNone,
	Strict = true,
	HasHeader = false
};
	
object[] data = new[]
{
	new object[] { 123, 123.45, 10M, "This is string", new DateTime(2010, 9, 3, 0, 0, 0), null },
	new object[] { 456, 456.78, 11M, "This is string too", new DateTime(2012, 04, 04, 0, 0, 0), null }
};
	
using (var memoryStream = new MemoryStream())
{
	using (TextWriter textWriter = new StreamWriter(memoryStream))
	{
		using (CSVWriter writer = new CSVWriter(dialect, textWriter))
		{
			foreach (object[] row in data)
			{
				writer.WriteRow(row);
			}
		}
	}
	
	Encoding encoding = Encoding.ASCII;
	Console.Write(encoding.GetString(memoryStream.ToArray()));
}
```

## Changes ##
 * 1.8.9.1102
  * Fixed issue #14 It would be nice to have a possibility to pass IFormatProbider for CsvWriter to convert objects to strings with desired format.
	Now you could provide CultureInfo into CsvWriter cunstructor and it will be used for formating of all Convertable(s) (IConvertible).
 * 1.7.10.1401
  * Integrated pull request: #15 Added multiline string and empty line skipping support by @emmorts
 * 0.9.21.1459
  * Fixed issue #8 "Property Headers in CSVReader is not initialised".
  * Refactored exceptions (all exceptions inherits from CsvException).
 * 0.8.0.1648
  * Fixed issue #6: Dialect doesn't support object initializer.
  * Fixed issue #5: If property 'HasHeader' is set to 'true', then CSVReader should not return header's row on method's 'GetCurrentRecord' invocation.
  * Cleaned code
  * Updated documentation
 * 0.7.2.1401
  * CSVReader method NextRecord renamed to Next.
  * CSVReader method GetCurrentRecord re-factored to property Current.
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


