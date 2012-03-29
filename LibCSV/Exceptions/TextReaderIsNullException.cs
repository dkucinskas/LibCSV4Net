using System;
using System.Runtime.Serialization;

namespace LibCSV.Exceptions
{
	[Serializable]
	public class TextReaderIsNullException : Exception
	{
		public TextReaderIsNullException()
			: base("TextReader is null! Set TextReader before reading records!")
		{ 
		}

		public TextReaderIsNullException(string message)
			: base(message)
		{
		}
		
		public TextReaderIsNullException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected TextReaderIsNullException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ 
		}
	}
}
