using System;
using System.Runtime.Serialization;

namespace LibCSV.Exceptions
{
	[Serializable]
	public class FileNameIsNullOrEmptyException : CsvException
	{
		public FileNameIsNullOrEmptyException()
			: base("CsvWriter: file name is null or empty!")
		{
		}

		public FileNameIsNullOrEmptyException(string message)
			: base(message)
		{
		}

		public FileNameIsNullOrEmptyException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected FileNameIsNullOrEmptyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
