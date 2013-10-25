using System;
using System.Runtime.Serialization;

namespace LibCSV.Exceptions
{
	[Serializable]
	public class CannotWriteToFileException : CsvException
	{
		public CannotWriteToFileException()
			: base("CsvWriter: file name is null or empty!")
		{
		}

		public CannotWriteToFileException(string message)
			: base(message)
		{
		}

		public CannotWriteToFileException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected CannotWriteToFileException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
