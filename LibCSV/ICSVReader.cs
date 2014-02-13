using System;

namespace LibCSV
{
	public interface ICSVReader : IDisposable
	{
		/// <summary>
		/// Reads and parses next record.
		/// </summary>
		/// <returns>true on success otherwise false.</returns>
		/// 
		bool Next();

		/// <summary>
		/// Returns the headers as string array.
		/// </summary>
		string[] Headers { get; }

		/// <summary>
		/// Returns the current record as string array.
		/// </summary>
		string[] Current { get; }
	}
}
