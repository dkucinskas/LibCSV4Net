using System;
using System.Collections.Generic;

namespace LibCSV
{
	public interface ICSVWriter : IDisposable
	{
		void WriteRow(IList<object> row);
	}
}
