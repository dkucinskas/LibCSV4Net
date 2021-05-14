using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibCSV
{
	public interface ICSVWriter : IDisposable
	{
		void WriteRow(IList<object> row);

		Task WriteRowAsync(IList<object> row);
	}
}
