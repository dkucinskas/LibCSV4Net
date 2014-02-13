using System;
using System.Collections.Generic;
using System.IO;
using LibCSV.Exceptions;

namespace LibCSV.Dialects
{
	/// <summary>
	/// CSVDialect holds CSV parsing and generation options.
	/// </summary>		
	public class Dialect : IDisposable
	{
		private bool _doubleQuote;  /* is " represented by ""? */
		
		private char _delimiter = '\0'; /* field separator */
		
		private char _quote; /* quote character */
		
		private char _escape = '\0';  /* escape character */
		
		private bool _skipInitialSpace = false; /* ignore spaces following delimiter? */
		
		private string _lineTerminator = null; /* string to write between records */
		
		private QuoteStyle _quoting = QuoteStyle.QuoteNone; /* style of quoting to write */
		
		private bool _strict; /* raise exception on bad CSV */
		
		private bool _hasHeader = false;
		
		private bool _disposed = false;
		
		public Dialect ()
			: this(true, ',', '"', '\0', false, "\r\n", QuoteStyle.QuoteMinimal, false, false)
		{
		}
		
		public Dialect(bool doubleQuote, char delimiter, char quote, char escape,
			bool skipInitialSpace, string lineTerminator, QuoteStyle quoting,
			bool strict, bool hasHeader)
		{
			_doubleQuote = doubleQuote;
			_delimiter = delimiter;
			_quote = quote;
			_escape = escape;
			_skipInitialSpace = skipInitialSpace;
			_lineTerminator = lineTerminator;
			_quoting = quoting;
			_strict = strict;
			_hasHeader = hasHeader;
			
			Check();
		}
		
		public void Check()
		{
			if (_delimiter == '\0')
			{
				throw new DialectInternalErrorException("Delimiter must be set");
			}
			
			if (_quoting != QuoteStyle.QuoteNone && _quote == '\0')
			{
				throw new DialectInternalErrorException("Quotechar must be set if quoting enabled");
			}
			
			if (_lineTerminator == null)
			{
				throw new DialectInternalErrorException("Line terminator must be set");
			}
		}
		
		public bool DoubleQuote
		{
			get { return _doubleQuote; }
			set { _doubleQuote = value; }
		}
		
		public string LineTerminator
		{
			get { return _lineTerminator; }
			set { _lineTerminator = value; }
		}
		
		public char Delimiter
		{
			get { return _delimiter; }
			set { _delimiter = value; }
		}
		
		public char Escape
		{
			get { return _escape; }
			set { _escape = value; }
		}
		
		public bool SkipInitialSpace
		{
			get { return _skipInitialSpace; }
			set { _skipInitialSpace = value; }
		}
		
		public char Quote
		{
			get { return _quote; }
			set { _quote = value; }
		}
		
		public QuoteStyle Quoting
		{
			get { return _quoting; }
			set { _quoting = value; }
		}
		
		public bool Strict
		{
			get { return _strict; }
			set { _strict = value; }
		}
		
		public bool HasHeader
		{
			get { return _hasHeader; }
			set { _hasHeader = value; }
		}

		public virtual ICSVReader CreateReader(string filename, string encoding)
		{
			return new CSVReader(this, filename, encoding);
		}

		public virtual ICSVReader CreateReader(TextReader reader)
		{
			return new CSVReader(this, reader);
		}

		public virtual ICSVWriter CreateWriter(string filename, string encoding)
		{
			return new CSVWriter(this, filename, encoding);
		}

		public virtual ICSVWriter CreateWriter(TextWriter writer)
		{
			return new CSVWriter(this, writer);
		}

		public bool IsDisposed
		{
			get { return _disposed; }
			private set { _disposed = value; }
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					_lineTerminator = null;
				}
				
				IsDisposed = true;
			}
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~Dialect()
		{
			Dispose(false);
		}
	}
}

