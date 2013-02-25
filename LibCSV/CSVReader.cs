using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibCSV.Dialects;
using LibCSV.Exceptions;

namespace LibCSV
{
    /// <summary>
    /// CSV reader is responsible for reading and parsing tabular data. 
    /// Parsing is controlled by set of rules defined in Dialect.
    /// API exposes the following operations: 
    ///  - Next() : reads and parses next record (returns true on success)
    ///  - Current : return current record as array of strings
    ///  - Headers : return headers as array of strings
    /// </summary>
	public class CSVReader : IDisposable
	{
		internal const int MAX_CAPACITY = 8000;// 8000 * 2 ~4GB max memory for process in 32bit OS

		internal const int DEFAULT_CAPACITY = 16;

		private TextReader _reader;

		private Dialect _dialect;

		private IList<string> _fields;

		private ParserState _state;

		private char[] _buffer;
		
        private int _capacity;

		private int _fieldLength;

		private bool _disposed;

	    private long _index;

	    private string[] _headers;

		public CSVReader(Dialect dialect, string filename, string encoding)
		{
			if (dialect != null)
				_dialect = dialect;

			GrowBuffer();

		    if (_reader != null) return;

		    if (!File.Exists(filename))
		        throw new ReaderException(string.Format("Can't read from file: '{0}', file not exists!", filename));

		    _reader = new StreamReader(filename, Encoding.GetEncoding(encoding));
		}

		public CSVReader(Dialect dialect, TextReader reader)
		{
			if (dialect != null)
				_dialect = dialect;

			GrowBuffer();

			if (reader != null)
				_reader = reader;
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

		protected void SaveField()
		{
			if (_buffer == null)
				throw new FieldIsNullException();

			_fields.Add(new string(_buffer, 0, _fieldLength));
			_fieldLength = 0;
			Array.Clear(_buffer, 0, _capacity);
		}

		protected void GrowBuffer()
		{
			if (_capacity == 0)
			{
				_capacity = DEFAULT_CAPACITY;
				_buffer = new char[_capacity];
			}
			else
			{
				_capacity *= 2;
				Array.Resize(ref _buffer, _capacity);
			}

			if (_buffer == null)
				throw new ReaderException("Can't grow buffer.");
		}

		protected void AddChar(char character)
		{
			if (_fieldLength >= MAX_CAPACITY)
				throw new ArgumentOutOfRangeException(string.Format("Field is larger than field limit ({0})", MAX_CAPACITY));

			if (_fieldLength == _capacity)
			{
				GrowBuffer();
			}

			_buffer[_fieldLength] = character;
			_fieldLength++;
		}

		private static bool IsNull(char character)
		{
			return (character == '\0');
		}

		private static bool IsEndOfLine(char character)
		{
			return (character == '\n' || character == '\r');
		}

		private static bool IsNullOrEndOfLine(char character)
		{
			return (IsNull(character) || IsEndOfLine(character));
		}

		protected void ProcessChar(char currentCharacter)
		{
			switch (_state)
			{
				case ParserState.StartOfRecord:
					{
						if (IsNull(currentCharacter))
						{
							break;
						}
					
                        if (IsEndOfLine(currentCharacter))
					    {
					        _state = ParserState.EndOfRecord;
					        break;
					    }

					    _state = ParserState.StartOfField;
						goto case ParserState.StartOfField;
					}

			    case ParserState.StartOfField:
					ProcessStartOfField(currentCharacter);
					break;

				case ParserState.EscapedCharacter:
					ProcessEscapedChar(currentCharacter);
					break;

				case ParserState.InField:
					ProcessInField(currentCharacter);
					break;

				case ParserState.InQuotedField:
					ProcessInQuotedField(currentCharacter);
					break;

				case ParserState.EscapeInQuotedField:
					ProcessEscapeInQuotedField(currentCharacter);
					break;

				case ParserState.QuoteInQuotedField:
					ProcessQuoteInQuotedField(currentCharacter);
					break;
			}
		}

		protected void ProcessQuoteInQuotedField(char currentCharacter)
		{
			if (_dialect.Quoting != QuoteStyle.QuoteNone && currentCharacter == _dialect.Quote)
			{
				AddChar(currentCharacter);
				_state = ParserState.InQuotedField;
			}
			else if (currentCharacter == _dialect.Delimiter)
			{
				SaveField();
				_state = ParserState.StartOfField;
			}
			else if (IsNullOrEndOfLine(currentCharacter))
			{
				SaveField();
				_state = (IsNull(currentCharacter) ? ParserState.StartOfRecord : ParserState.EndOfRecord);
			}
			else if (_dialect.Strict == false)
			{
				AddChar(currentCharacter);
				_state = ParserState.InField;
			}
			else
			{
				throw new BadFormatException(
					string.Format("Bad format: '{0}' expected after '{1}'", _dialect.Delimiter, _dialect.Quote));
			}
		}

		protected void ProcessEscapeInQuotedField(char currentCharacter)
		{
			if (IsNull(currentCharacter))
				currentCharacter = '\n';

			AddChar(currentCharacter);
			_state = ParserState.InQuotedField;
		}

		protected void ProcessInQuotedField(char currentCharacter)
		{
			if (IsNull(currentCharacter))
			{
			}
			else if (currentCharacter == _dialect.Escape)
			{
				_state = ParserState.EscapeInQuotedField;
			}
			else if (currentCharacter == _dialect.Quote && _dialect.Quoting != QuoteStyle.QuoteNone)
			{
				_state = _dialect.DoubleQuote ? ParserState.QuoteInQuotedField : ParserState.InField;
			}
			else
			{
				AddChar(currentCharacter);
			}
		}

		protected void ProcessInField(char currentCharacter)
		{
			if (IsNullOrEndOfLine(currentCharacter))
			{
				SaveField();
				_state = (IsNull(currentCharacter) ? ParserState.StartOfRecord : ParserState.EndOfRecord);
			}
			else if (currentCharacter == _dialect.Escape)
			{
				_state = ParserState.EscapedCharacter;
			}
			else if (currentCharacter == _dialect.Delimiter)
			{
				SaveField();
				_state = ParserState.StartOfField;
			}
			else
			{
				AddChar(currentCharacter);
			}
		}

		protected void ProcessEscapedChar(char currentCharacter)
		{
			if (IsNull(currentCharacter))
				currentCharacter = '\n';

			AddChar(currentCharacter);
			_state = ParserState.InField;
		}

		protected void ProcessStartOfField(char currentCharacter)
		{
			if (IsNullOrEndOfLine(currentCharacter))
			{
				SaveField();
				_state = (IsNull(currentCharacter) ? ParserState.StartOfRecord : ParserState.EndOfRecord);
			}
			else if (currentCharacter == _dialect.Quote && _dialect.Quoting != QuoteStyle.QuoteNone)
			{
				_state = ParserState.InQuotedField;
			}
			else if (currentCharacter == _dialect.Escape)
			{
				_state = ParserState.EscapedCharacter;
			}
			else if (char.IsWhiteSpace(currentCharacter) && _dialect.SkipInitialSpace)
			{
			}
			else if (currentCharacter == _dialect.Delimiter)
			{
				SaveField();
			}
			else
			{
				AddChar(currentCharacter);
				_state = ParserState.InField;
			}
		}

		protected void Reset()
		{
			_fields = new List<string>();
			_fieldLength = 0;
			_state = ParserState.StartOfRecord;
		}

		public bool Next()
		{
			if (_reader == null)
				throw new TextReaderIsNullException();

			if (_dialect == null)
				throw new DialectIsNullException();

			if (!string.IsNullOrEmpty(_dialect.Error))
				throw new DialectInternalErrorException("Dialect error: " + _dialect.Error);

		    Reset();

			var line = _reader.ReadLine();
			if (string.IsNullOrEmpty(line))
				return false;

			var length = line.Length;
			if (length < 0)
				return false;

			for (var i = 0; i < length; i++)
			{
				var c = line[i];
				if (IsNull(c))
					throw new BadFormatException("Line contains NULL byte!");

				ProcessChar(c);
			}
            SaveField();

		    _index++;
			return true;
		}

	    public string[] Headers
	    {
	        get
	        {
	            return _headers;
	        }
	    }

	    public string[] Current
	    {
	        get
	        {
	            if (_fields == null)
	            {
	                return null;
	            }

	            string[] results = null;

                if (_index == 1 && _dialect.HasHeader)
                {
                    if (_headers == null)
                    {
                        _headers = new string[_fields.Count];
                        _fields.CopyTo(_headers, 0);
                    }
                }
                else
                {
                    results = new string[_fields.Count];
                    _fields.CopyTo(results, 0);                    
                }

	            return results;
	        }
	    }

	    protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					if (_reader != null)
					{
						_reader.Dispose();
					}
					_reader = null;

					_dialect = null;
					_fields = null;
					_buffer = null;
				}

				IsDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~CSVReader()
		{
			Dispose(false);
		}
	}
}
