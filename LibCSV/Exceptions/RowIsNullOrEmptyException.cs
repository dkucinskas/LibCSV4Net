using System;
using System.Runtime.Serialization;

namespace LibCSV.Exceptions
{
	[Serializable]
	public class RowIsNullOrEmptyException : CsvException
	{
		public RowIsNullOrEmptyException()
			: base("CsvWriter: row is null or empty!")
		{
		}

		public RowIsNullOrEmptyException(string message)
			: base(message)
		{
		}

		public RowIsNullOrEmptyException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected RowIsNullOrEmptyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
