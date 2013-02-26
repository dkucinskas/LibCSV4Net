using System;
using System.Collections.Generic;

namespace LibCSV.Dialects
{
	/// <summary>
	/// CSVDialect holds CSV parsing and generation options.
	/// </summary>		
	public class Dialect : IDisposable
	{
		public static IList<StyleDesc> QuoteStyles = new List<StyleDesc>()
		{
			new StyleDesc(QuoteStyle.QuoteMinimal, "QUOTE_MINIMAL"),
			new StyleDesc(QuoteStyle.QuoteAll, "QUOTE_ALL"),
			new StyleDesc(QuoteStyle.QuoteNonnumeric, "QUOTE_NONNUMERIC"),
			new StyleDesc(QuoteStyle.QuoteNone, "QUOTE_NONE")
		};

		private bool _doubleQuote;  /* is " represented by ""? */

		private char _delimiter = '\0'; /* field separator */

		private char _quote; /* quote character */

		private char _escape = '\0';  /* escape character */

		private bool _skipInitialSpace = false; /* ignore spaces following delimiter? */

		private string _lineTerminator = null; /* string to write between records */

		private QuoteStyle _quoting = QuoteStyle.QuoteNone; /* style of quoting to write */

		private bool _strict;                 /* raise exception on bad CSV */

		private string _error = null;

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

			if (_delimiter == '\0')
				_error = "Delimiter must be set";

			if (CheckQuoting() == false)
				_error = "Bad \"quoting\" value";

			if (_quoting != QuoteStyle.QuoteNone && _quote == '\0')
				_error = "Quotechar must be set if quoting enabled";

			if (_lineTerminator == null)
				_error = "Line terminator must be set";
		}

		public bool DoubleQuote
		{
			get { return _doubleQuote; }
			protected set { _doubleQuote = value; }
		}

		public string LineTerminator
		{
			get { return _lineTerminator; }
			protected set { _lineTerminator = value; }
		}

		public char Delimiter
		{
			get { return _delimiter; }
			protected set { _delimiter = value; }
		}

		public char Escape
		{
			get { return _escape; }
			protected set { _escape = value; }
		}

		public bool SkipInitialSpace
		{
			get { return _skipInitialSpace; }
			protected set { _skipInitialSpace = value; }
		}

		public char Quote
		{
			get { return _quote; }
			protected set { _quote = value; }
		}

		public QuoteStyle Quoting
		{
			get { return _quoting; }
			protected set { _quoting = value; }
		}

		public bool Strict
		{
			get { return _strict; }
			protected set { _strict = value; }
		}

		public string Error
		{
			get { return this._error; }
			protected set { _error = value; }
		}

		public bool HasHeader
		{
			get { return _hasHeader; }
			set { _hasHeader = value; }
		}

		public bool IsDisposed
		{
			get { return _disposed; }
			private set { _disposed = value; }
		}

		public bool CheckQuoting()
		{
			var count = QuoteStyles.Count;
			for (var i = 0; i < count; i++)
			{
				if (QuoteStyles[i].Style == _quoting)
					return true;
			}

			return false;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					_lineTerminator = null;
					_error = null;
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

