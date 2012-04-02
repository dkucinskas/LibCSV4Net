using System;
using System.Runtime.Serialization;

namespace LibCSV.Exceptions
{
	[Serializable]
	public class DialectIsNullException : Exception
	{
		public DialectIsNullException()
			: base("Dialect is null! Set Dialect before parssing!")
		{
		}

		public DialectIsNullException(string message)
			: base(message)
		{
		}

		public DialectIsNullException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DialectIsNullException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
