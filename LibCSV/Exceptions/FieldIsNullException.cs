using System;
using System.Runtime.Serialization;

namespace LibCSV.Exceptions
{
	[Serializable]
	public class FieldIsNullException : CsvException
	{
		public FieldIsNullException()
			: base("Field is null!")
		{
		}

		public FieldIsNullException(string message)
			: base(message)
		{
		}

		public FieldIsNullException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected FieldIsNullException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
