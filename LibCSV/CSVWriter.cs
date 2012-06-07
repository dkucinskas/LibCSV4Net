using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibCSV.Dialects;
using LibCSV.Exceptions;

namespace LibCSV
{
    /// <summary>
    /// CSV Writer. Writes tabula data to stream.
    /// </summary>
	public class CSVWriter : IDisposable
	{
		private TextWriter _writer;

	    private Dialect _dialect;

		private bool _disposed;

		public CSVWriter()
		{
		}

		public CSVWriter(Dialect dialect, string filename, string encoding)
		{
			if (dialect != null)
				_dialect = dialect;

			if (_writer == null)
			{
				if (filename.Trim().Length > 0)
					if (!File.Exists(filename))
						throw new WriterException(string.Format("Can't write to file: '{0}', file not exists!", filename));

				_writer = new StreamWriter(filename, false, Encoding.GetEncoding(encoding));
			}
		}

		public CSVWriter(Dialect dialect, TextWriter writer)
		{
			if (dialect != null)
				_dialect = dialect;

			if (writer != null)
				_writer = writer;
		}

		public Dialect Dialect
		{
			get { return _dialect; }
			set { _dialect = value; }
		}

		public bool IsDisposed
		{
			get { return _disposed; }
			private set { _disposed = value; }
		}

		public void WriteRow(IList<object> row)
		{
			if (Dialect == null)
				throw new DialectIsNullException("Set dialect first!");

			if (row == null || row.Count < 1)
				return;

			var count = row.Count;
			for (var i = 0; i < count; i++)
			{
				WriteField(row[i]);

				if (i != count - 1)
					_writer.Write(Dialect.Delimiter);
			}

			_writer.WriteLine();
			row = null;
		}

		protected virtual void WriteField(object field)
		{
			if (field == null)
			{
			}
			else if (field is string)
			{
				WriteString((string)field);
			}
			else if (field is int || field is long ||
					 field is double || field is float ||
					 field is decimal)
			{
				WriteNumber(field);
			}
			else
			{
				WriteObject(field);
			}
		}

		protected virtual void WriteString(string field)
		{
			if (Dialect.Quoting == QuoteStyle.QuoteNone)
				_writer.Write(field);
			else
			{
				_writer.Write(Dialect.Quote);

				if (Dialect.DoubleQuote)
					WriteEscapedString(field);
				else
					WriteString(field);

				_writer.Write(Dialect.Quote);
			}
		}

		protected virtual void WriteNumber(object field)
		{
			if (Dialect.Quoting == QuoteStyle.QuoteAll)
				_writer.Write(Dialect.Quote);

			_writer.Write(field);

			if (Dialect.Quoting == QuoteStyle.QuoteAll)
				_writer.Write(Dialect.Quote);
		}

		protected virtual void WriteObject(object field)
		{
			if (Dialect.Quoting == QuoteStyle.QuoteAll)
				_writer.Write(Dialect.Quote);

			_writer.Write(field.ToString());

			if (Dialect.Quoting == QuoteStyle.QuoteAll)
				_writer.Write(Dialect.Quote);
		}

		protected virtual void WriteEscapedString(string field)
		{
			var count = field.Length;
			for (var i = 0; i < count; i++)
			{
				if (field[i] == Dialect.Quote)
					_writer.Write(Dialect.Escape);

				_writer.Write(field[i]);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					_dialect = null;
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
