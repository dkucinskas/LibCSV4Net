using System;
using System.Runtime.Serialization;

namespace LibCSV.Exceptions
{
	[Serializable]
	public class BadFormatException : Exception
	{
		public BadFormatException()
			: base("Bad format!")
		{ 
		}

		public BadFormatException(string message)
			: base(message)
		{
		}
		
		public BadFormatException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected BadFormatException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ 
		}
	}
}
