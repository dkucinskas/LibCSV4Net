using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibCSV
{
	public interface ICSVWriter : IDisposable
	{
		void Open();

		Task OpenAsync();

		void WriteRow(IList<object> row);

		Task WriteRowAsync(IList<object> row);
	}
}
