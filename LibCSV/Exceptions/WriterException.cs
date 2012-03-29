using System;
using System.Runtime.Serialization;

namespace LibCSV
{
	[Serializable]
	public class WriterException : Exception
	{
		public WriterException()
		{
		}
		
		public WriterException(string message) 
			: base (message)
		{
		}
		
		public WriterException(string message, Exception inner) 
			: base (message, inner)
		{
		}
		
		protected WriterException(SerializationInfo info, StreamingContext context) 
			: base (info, context)
		{
			
		}
	}
}
