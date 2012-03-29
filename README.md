# LibCSV

Library for reading and writing tabular (CSV) files.

## Cahnges

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

 * Define your Dialect

``` c#
public class MyDialect : Dialect
{
	public MyDialect()
	{
		DoubleQuote = true;
		Delimiter = ';';
		Quote = '"';
		Escape = '\0';
		SkipInitialSpace = false;
		LineTerminator = "\r\n";
		Quoting = QuoteStyle.QUOTE_NONNUMERIC;
		Strict = false;
		
		if (CheckQuoting() == false)
			throw new Exception("bad \"quoting\" value");
	}
}
```

 * Now you can use it like this

``` c#
using (CSVReader csvReader = new CSVReader(new MyDialect(), @"C:\CSV\MY_TEST.CSV", "windows-1257"))
{
	while (csvReader.NextRecord())
	{
		string[] record = csvReader.GetCurrentRecord();
		foreach(string item in record)
		{
			Console.Write(" " + item);
		}
		Console.WriteLine();
	}
}
```

## License

This is free software, and you are welcome to redistribute it under certain conditions; see LICENSE.txt for details.

## Author contact

Darius Kucinskas d.kucinskas@gmail.com, http://blog-of-darius.blogspot.com/

