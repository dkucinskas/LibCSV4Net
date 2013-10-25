using System;
using System.Runtime.Serialization;

namespace LibCSV.Exceptions
{
	[Serializable]
	public class CsvException : Exception
	{
		public CsvException()
			: base("Exception occurred!")
		{
		}

		public CsvException(string message)
			: base(message)
		{
		}

		public CsvException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected CsvException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

