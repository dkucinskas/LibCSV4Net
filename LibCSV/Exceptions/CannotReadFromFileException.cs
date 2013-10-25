using System;
using System.Runtime.Serialization;

namespace LibCSV.Exceptions
{
	[Serializable]
	public class CannotReadFromFileException : CsvException
	{
		public CannotReadFromFileException()
			: base("CsvReader: can't read from file!")
		{
		}

		public CannotReadFromFileException(string message)
			: base(message)
		{
		}

		public CannotReadFromFileException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected CannotReadFromFileException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
