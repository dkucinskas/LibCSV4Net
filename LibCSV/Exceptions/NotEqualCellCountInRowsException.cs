using System;
using System.Runtime.Serialization;

namespace LibCSV
{
	public class NotEqualCellCountInRowsException : CsvException
	{
		public NotEqualCellCountInRowsException(int length, int cellCount)
			: base(
				string.Format("Cell count in all rows must be equal. Tried insert row with cell count {0}, but previously inserted row or header with cell count {1}.",
			              length, cellCount))
		{
		}

		public NotEqualCellCountInRowsException() : 
			base("Not equal cell count in rows!")
		{
		}

		public NotEqualCellCountInRowsException(string message)
			: base(message)
		{
		}

		public NotEqualCellCountInRowsException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected NotEqualCellCountInRowsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

