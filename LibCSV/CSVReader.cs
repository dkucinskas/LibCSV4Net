using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibCSV.Dialects;
using LibCSV.Exceptions;

namespace LibCSV
{
	/// <summary>
	/// CSVReader class is responsible for reading and parsing tabular data.
	/// Parsing is controlled by set of rules defined in Dialect.
	/// API exposes the following operations:
	/// Next() : reads and parses next record (returns true on success)
	/// Current : return current record as array of strings
	/// Headers : return headers as array of strings
	/// </summary>
	public class CSVReader : ICSVReader
	{
		internal const int DEFAULT_CAPACITY = 16;
		
		private TextReader _reader;
		
		private Dialect _dialect;
		
		private IList<string> _fields;
		
		private ParserState _state;
		
		private char[] _buffer;
		
		private int _capacity;
		
		private int _fieldLength;
		
		private long _index;
		
		private string[] _headers;
		
		private bool _ownsReader = false;
		
		public CSVReader(Dialect dialect, string filename, string encoding)
		{
			if (dialect == null) 
			{
				throw new DialectIsNullException();
			}
			_dialect = dialect;
			
			GrowBuffer();
			
			if (_reader == null)
			{
				if (string.IsNullOrEmpty(filename) || filename.Trim().Length < 1)
				{
					throw new FileNameIsNullOrEmptyException();
				}
				
				if (!File.Exists(filename))
				{
					throw new CannotReadFromFileException(string.Format("Can't read from file: '{0}', file not exists!", filename));
				}
				
				_ownsReader = true;
				try
				{
					_reader = new StreamReader(filename, Encoding.GetEncoding(encoding));
				}
				catch(Exception exp)
				{
					throw new CannotReadFromFileException(string.Format("Can't read from file: '{0}'!", filename), exp);
				}
			}
			
			InitializeHeaders();
		}
		
		public CSVReader(Dialect dialect, TextReader reader)
		{
			if (dialect == null) 
			{
				throw new DialectIsNullException();
			}
			_dialect = dialect;
			
			GrowBuffer();
			
			if (reader == null)
			{
				throw new TextReaderIsNullException();
			}
			_reader = reader;
			
			InitializeHeaders();
		}
		
		public bool IsDisposed { get; private set; }
		
		protected void InitializeHeaders()
		{
			if (_dialect != null && _dialect.HasHeader) 
			{
				Next();
				
				if (_headers == null && _fields != null && _fields.Count > 0)
				{
					_headers = new string[_fields.Count];
					_fields.CopyTo(_headers, 0);
				}
			}
		}
		
		protected void SaveField()
		{
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
		}
		
		protected void AddChar(char character)
		{
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
						//TODO: should we add ignore null byte attribute to dialect?
//						if (IsNull(currentCharacter))
//						{
//							break;
//						}
						
						//TODO: can this be executed?
//						if (IsEndOfLine(currentCharacter))
//						{
//							_state = ParserState.EndOfRecord;
//							break;
//						}
						
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
			else if (!_dialect.Strict) 
			{
				AddChar (currentCharacter);
				_state = ParserState.InField;
			} else {
				throw new BadFormatException (
					string.Format ("Bad format: '{0}' expected after '{1}'", _dialect.Delimiter, _dialect.Quote));
			}
		}
		
		protected void ProcessEscapeInQuotedField(char currentCharacter)
		{
			if (IsNull(currentCharacter) || currentCharacter == 'n')
				currentCharacter = '\n';

			if (currentCharacter == 'r')
				currentCharacter = '\r';

			if (currentCharacter == 't')
				currentCharacter = '\t';

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
		
		/// <summary>
		/// Reads and parses next record.
		/// </summary>
		/// <returns>true on success otherwise false.</returns>
		public bool Next()
		{
			Reset();
			
			var line = ReadLine();
			if (string.IsNullOrEmpty(line) || line.Trim().Length < 1)
			{
				return false;
			}

			var length = line.Length;
			if (line != new string(_dialect.Delimiter, length))
			{
				for (var i = 0; i < length; i++)
				{
					if (IsNull(line[i]))
					{
						throw new BadFormatException("Line contains NULL byte!");
					}

					ProcessChar(line[i]);
				}

				SaveField();
			}
			
			_index++;
			return true;
		}

		/// <summary>
		/// Returns the next line.
		/// </summary>
		public virtual String ReadLine()
		{
			StringBuilder sb = new StringBuilder();
			bool inQuotes = false;
			while (true)
			{
				int ch = _reader.Read();
				if (ch == -1) break;
				if (ch == _dialect.Quote) inQuotes = !inQuotes;
				if (!inQuotes && (ch == '\r' || ch == '\n'))
				{
					if (ch == '\r' && _reader.Peek() == '\n') _reader.Read();
					return sb.ToString();
				}
				sb.Append((char)ch);
			}
			if (sb.Length > 0) return sb.ToString();
			return null;
		} 
		
		/// <summary>
		/// Returns the headers as string array.
		/// </summary>
		public string[] Headers
		{
			get
			{
				return _headers;
			}
		}
		
		/// <summary>
		/// Returns the current record as string array.
		/// </summary>
		public string[] Current
		{
			get
			{
				string[] results = null;
				
				if (_fields != null)
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
					if (_ownsReader && _reader != null)
					{
						_reader.Close();
						_reader = null;
					}
					
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
