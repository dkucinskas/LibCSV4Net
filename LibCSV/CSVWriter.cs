using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibCSV.Dialects;
using LibCSV.Exceptions;

namespace LibCSV
{
	/// <summary>
	/// CSVWriter class is responsible for writing tabular data to stream.
	/// </summary>
	public class CSVWriter : ICSVWriter
	{
		private TextWriter _writer;
		
		private Dialect _dialect;
		
		private bool _disposed;
		
		private bool _ownsWriter = false;
		
		public CSVWriter(Dialect dialect, string filename, string encoding)
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
		}
		
		public CSVWriter(Dialect dialect, TextWriter writer)
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
		}
		
		public bool IsDisposed
		{
			get { return _disposed; }
			private set { _disposed = value; }
		}
		
		public void WriteRow(IList<object> row)
		{
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
		
		protected virtual void WriteField(object field)
		{
			if (field == null)
			{
			}
			else if (field is string)
			{
				WriteString((string)field);
			}
			else if (field is int ||
					field is long ||
					field is double || 
					field is float ||
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
					_writer.Write(field);
				}
				
				_writer.Write(_dialect.Quote);
			}
		}
		
		protected virtual void WriteNumber(object field)
		{
			if (_dialect.Quoting == QuoteStyle.QuoteAll)
			{
				_writer.Write(_dialect.Quote);
			}
			
			_writer.Write(field);
			
			if (_dialect.Quoting == QuoteStyle.QuoteAll)
			{
				_writer.Write(_dialect.Quote);
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
		
		protected virtual void WriteEscapedString(string field)
		{
			var count = field.Length;
			for (var i = 0; i < count; i++)
			{
				if (field[i] == _dialect.Quote)
				{
					_writer.Write(_dialect.Escape);
				}
				
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
					if (_ownsWriter && _writer != null)
					{
						_writer.Close();
						_writer = null;
					}
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
