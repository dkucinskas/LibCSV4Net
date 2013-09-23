using System;
using System.Runtime.Serialization;

namespace LibCSV
{
	public class DataTransformerIsNullException : CsvException
	{
		public DataTransformerIsNullException() : 
			base("DataTransformer is undefined! Set DataTransformer before parssing!")
		{
		}

		public DataTransformerIsNullException(string message)
			: base(message)
		{
		}

		public DataTransformerIsNullException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DataTransformerIsNullException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

