# Changes

## 2.1.0.0

  * Ported to .NETStandart 2.0

## 2.0.0.0

  * Ported to .NETStandart 2.0

## 1.8.9.1102

  * Fixed issue #14 It would be nice to have a possibility to pass IFormatProbider for CsvWriter to convert objects to strings with desired format.
  * Now you could provide CultureInfo into CsvWriter cunstructor and it will be used for formating of all Convertable(s) (IConvertible).

## 1.7.10.1401

  * Integrated pull request: #15 Added multiline string and empty line skipping support by @emmorts

## 0.9.21.1459

  * Fixed issue #8 "Property Headers in CSVReader is not initialised".
  * Refactored exceptions (all exceptions inherits from CsvException).

## 0.8.0.1648

  * Fixed issue #6: Dialect doesn't support object initializer.
  * Fixed issue #5: If property 'HasHeader' is set to 'true', then CSVReader should not return header's row on method's 'GetCurrentRecord' invocation.
  * Cleaned code
  * Updated documentation

## 0.7.2.1401

  * CSVReader method NextRecord renamed to Next.
  * CSVReader method GetCurrentRecord re-factored to property Current.
  * Updated documentation to reflect this change.

## 0.6.8.1105

  * Added NuGet package

## 0.6.7.0838

  * Code cleanup
  * Headers must be specified explicitly in CSVDialect
  * Updated examples in README.md

## 0.5

  * Improved CSVAdapter
  * Transformer supports transforming result for writing

## 0.4

  * Added CSVWriter

## 0.3

  * Changed IResultTransformer interface
  * IList TransformList(IList result) -> IEnumerable TransformResult(IEnumerable result);

## 0.2

  * Redesigned CSVReader interface
    * Dialect must be set in constructor (removed property)
    * Added header support to Dialect
    * Added GetCurrentRecord
    * Record is returned as string array
  * Added IResultTransformer
  * Added Adapter class