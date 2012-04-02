using System;
using System.Runtime.Serialization;

namespace LibCSV.Exceptions
{
	[Serializable]
	public class DialectInternalErrorException : Exception
	{
		public DialectInternalErrorException()
			: base("Dialect internal error!")
		{
		}

		public DialectInternalErrorException(string message)
			: base(message)
		{
		}

		public DialectInternalErrorException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DialectInternalErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
