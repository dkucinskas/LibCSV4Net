using System;
using System.Runtime.Serialization;

namespace LibCSV.Exceptions
{
	[Serializable]
	public class HeaderIsNullException : CsvException
	{
		public HeaderIsNullException()
			: base("Header is undefined!")
		{
		}

		public HeaderIsNullException(string message)
			: base(message)
		{
		}

		public HeaderIsNullException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected HeaderIsNullException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

