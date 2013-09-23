using System;
using System.Runtime.Serialization;

namespace LibCSV
{
	public class CannotConvertValueToRequestType : CsvException
	{
		public CannotConvertValueToRequestType(string value, Type actualType, Type requestedType)
			: base(
				string.Format("Cannot convert value '{0}' of type '{1}' to request type '{2}'",
			              value,
			              actualType,
			              requestedType))
		{
		}

		public CannotConvertValueToRequestType() : 
			base("Not equal cell count in rows!")
		{
		}

		public CannotConvertValueToRequestType(string message)
			: base(message)
		{
		}

		public CannotConvertValueToRequestType(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected CannotConvertValueToRequestType(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

