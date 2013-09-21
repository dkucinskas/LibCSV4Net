using System;
using System.Runtime.Serialization;

namespace LibCSV.Exceptions
{
	[Serializable]
	public class ReaderException : CsvException
	{
		public ReaderException()
			: base("CSVReader internal error!")
		{
		}

		public ReaderException(string message)
			: base(message)
		{
		}

		public ReaderException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected ReaderException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
