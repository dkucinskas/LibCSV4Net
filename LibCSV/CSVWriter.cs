using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibCSV.Dialects;
using LibCSV.Exceptions;

namespace LibCSV
{
	/// <summary>
	/// CSVWriter class is responsible for writing tabular data to stream.
	/// </summary>
	public class CSVWriter : ICSVWriter
	{
		private bool _opened = false;

		private TextWriter _writer;

		private Dialect _dialect;

		private bool _ownsWriter = false;

		private CultureInfo _culture = CultureInfo.InvariantCulture;

		public bool IsDisposed { get; private set; }

		public CSVWriter(Dialect dialect, string filename, string encoding, CultureInfo culture = null)
		{
			if (dialect == null)
			{
				throw new DialectIsNullException("Set dialect first!");
			}
			dialect.Check();
			_dialect = dialect;

			if (_writer == null)
			{
				if (string.IsNullOrEmpty(filename) || filename.Trim().Length < 1)
				{
					throw new FileNameIsNullOrEmptyException();
				}

				if (!File.Exists(filename))
				{
					throw new CannotWriteToFileException(string.Format("Can't write to file: '{0}', file not exists!", filename));
				}

				_ownsWriter = true;
				try
				{
					_writer = new StreamWriter(filename, false, Encoding.GetEncoding(encoding));
				}
				catch(Exception exp)
				{
					throw new CannotWriteToFileException(string.Format("Can't write to file: '{0}'!", filename), exp);
				}
			}

			_culture = culture ?? Thread.CurrentThread.CurrentCulture;
		}

		public CSVWriter(Dialect dialect, TextWriter writer, CultureInfo culture = null)
		{
			if (dialect == null)
			{
				throw new DialectIsNullException("Set dialect first!");
			}
			dialect.Check();
			_dialect = dialect;

			if (writer != null)
			{
				_writer = writer;
			}

			_culture = culture ?? Thread.CurrentThread.CurrentCulture;
		}

		public void Open() { _opened = true; }

		public async Task OpenAsync()
		{ 
			_opened = true;

			await Task.CompletedTask;
		}

		public void WriteRow(IList<object> row)
		{
			ThrowIfClosed();

			if (row == null || row.Count < 1)
			{
				throw new RowIsNullOrEmptyException();
			}

			var count = row.Count;
			for (var i = 0; i < count; i++)
			{
				WriteField(row[i]);

				if (i != count - 1)
				{
					_writer.Write(_dialect.Delimiter);
				}
			}

			_writer.Write(_dialect != null ? _dialect.LineTerminator : Environment.NewLine);
		}

		public async Task WriteRowAsync(IList<object> row)
		{
			ThrowIfClosed();

			if (row == null || row.Count < 1)
			{
				throw new RowIsNullOrEmptyException();
			}

			var count = row.Count;
			for (var i = 0; i < count; i++)
			{
				await WriteFieldAsync(row[i]);

				if (i != count - 1)
				{
					await _writer.WriteAsync(_dialect.Delimiter);
				}
			}

			await _writer.WriteAsync(_dialect != null ? _dialect.LineTerminator : Environment.NewLine);
		}

		protected virtual void WriteField(object field)
		{
			if (field == null) 
			{
				return;
			}

			if (field is string str)
			{
				 WriteString(str);
			}
			else if (field is IConvertible convertible)
			{
				WriteConvertible(convertible);
			}
			else
			{
				WriteObject(field);
			}
		}

		protected virtual async Task WriteFieldAsync(object field)
		{
			if (field == null)
			{
				return;
			}

			if (field is string str)
            {
				await WriteStringAsync(str);
			}
			else if (field is IConvertible convertible)
            {
				await WriteConvertibleAsync(convertible);
			}
			else
            {
				await WriteObjectAsync(field);
			}
		}

		protected virtual void WriteString(string field)
		{
			if (_dialect.Quoting == QuoteStyle.QuoteNone)
			{
				_writer.Write(field);
			}
			else
			{
				_writer.Write(_dialect.Quote);

				if (_dialect.DoubleQuote)
				{
					WriteEscapedString(field);
				}
				else
				{
					_writer.Write(field.ToString(_culture));
				}

				_writer.Write(_dialect.Quote);
			}
		}

		protected virtual async Task WriteStringAsync(string field)
		{
			if (_dialect.Quoting == QuoteStyle.QuoteNone)
			{
				await _writer.WriteAsync(field);
			}
			else
			{
				await _writer.WriteAsync(_dialect.Quote);

				if (_dialect.DoubleQuote)
				{
					await WriteEscapedStringAsync(field);
				}
				else
				{
					await _writer.WriteAsync(field.ToString(_culture));
				}

				await _writer.WriteAsync(_dialect.Quote);
			}
		}

		protected virtual void WriteConvertible<T>(T field)
			where T : IConvertible
		{
			if (_dialect.Quoting == QuoteStyle.QuoteAll)
			{
				_writer.Write(_dialect.Quote);
			}

			var convertible = field as  IConvertible;
			_writer.Write (convertible.ToString(_culture));

			if (_dialect.Quoting == QuoteStyle.QuoteAll)
			{
				_writer.Write(_dialect.Quote);
			}
		}

		protected virtual async Task WriteConvertibleAsync(IConvertible field)
		{
			if (_dialect.Quoting == QuoteStyle.QuoteAll)
			{
				await _writer.WriteAsync(_dialect.Quote);
			}

			await _writer.WriteAsync(field.ToString(_culture));

			if (_dialect.Quoting == QuoteStyle.QuoteAll)
			{
				await _writer.WriteAsync(_dialect.Quote);
			}
		}

		protected virtual void WriteObject(object field)
		{
			if (_dialect.Quoting == QuoteStyle.QuoteAll)
			{
				_writer.Write(_dialect.Quote);
			}

			_writer.Write(field.ToString());

			if (_dialect.Quoting == QuoteStyle.QuoteAll)
			{
				_writer.Write(_dialect.Quote);
			}
		}

		protected virtual async Task WriteObjectAsync(object field)
		{
			if (_dialect.Quoting == QuoteStyle.QuoteAll)
			{
				await _writer.WriteAsync(_dialect.Quote);
			}

			await _writer.WriteAsync(field.ToString());

			if (_dialect.Quoting == QuoteStyle.QuoteAll)
			{
				await _writer.WriteAsync(_dialect.Quote);
			}
		}

		protected virtual void WriteEscapedString(string field)
		{
			var count = field.Length;
			for (var i = 0; i < count; i++)
			{
				if (field[i] == _dialect.Quote)
				{
					_writer.Write(_dialect.Escape);
				}

				_writer.Write(field[i].ToString(_culture));
			}
		}

		protected virtual async Task WriteEscapedStringAsync(string field)
		{
			var count = field.Length;
			for (var i = 0; i < count; i++)
			{
				if (field[i] == _dialect.Quote)
				{
					await _writer.WriteAsync(_dialect.Escape);
				}

				await _writer.WriteAsync(field[i].ToString(_culture));
			}
		}

		protected void ThrowIfClosed()
		{
			if (!_opened || IsDisposed)
			{
				throw new CsvException("CSV writer is closed");
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					_dialect = null;
					if (_ownsWriter && _writer != null)
					{
						_writer.Close();
						_writer = null;
					}

					_culture = null;
				}

				IsDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~CSVWriter()
		{
			Dispose(false);
		}
	}
}
