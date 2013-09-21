using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using LibCSV.Dialects;

namespace LibCSV
{
	/// <summary>
	/// CSVAdapter class is advanced csv reader/writer that supports read/write of all records and transformations.
	/// </summary>
	public class CSVAdapter : IDisposable
	{
		private bool _disposed;

		private string _filename;

		private string _encoding;

		private TextReader _reader;

		private TextWriter _writer;

		private Dialect _dialect;

		private IDictionary<string, Type> _types = new Dictionary<string, Type>();

	    private string[] _headers;

		public CSVAdapter(Dialect dialect, string filename, string encoding)
		{
			_dialect = dialect;
			_filename = filename;
			_encoding = encoding;
		}

		public CSVAdapter(Dialect dialect, TextReader reader)
		{
			_dialect = dialect;
			_reader = reader;
		}

		public CSVAdapter(Dialect dialect, TextWriter writer)
		{
			_dialect = dialect;
			_writer = writer;
		}

        public CSVAdapter(Dialect dialect, TextWriter writer, string[] headers)
        {
            _dialect = dialect;
            _writer = writer;

			if (_dialect.HasHeader && headers == null) 
			{
				throw new HeaderIsNullException("Provided header array is not initialized!");
			}

            _headers = headers;
        }

		public bool IsDisposed
		{
			get { return _disposed; }
			private set { _disposed = value; }
		}

		public void SetColumn(string alias, Type type)
		{
			_types.Add(alias, type);
		}

		private CSVReader CreateReader()
		{
		    return _reader == null ? 
                new CSVReader(_dialect, _filename, _encoding) : 
                new CSVReader(_dialect, _reader);
		}

	    private CSVWriter CreateWriter()
	    {
	        return _writer != null ? 
                new CSVWriter(_dialect, _writer) : 
                new CSVWriter(_dialect, _filename, _encoding);
	    }

	    public IEnumerable ReadAll(IDataTransformer transformer)
		{
			IList results = new ArrayList();

			var reader = CreateReader();

			var row = 0;
			string[] aliases = null;
			if (_dialect.HasHeader) 
			{
				aliases = reader.Headers;
				if (aliases == null) 
				{
					throw new HeaderIsNullException("Failed to read headers");
				}
			}

			while (reader.Next())
			{
				var values = reader.Current;

				if (_types.Count > 0)
				{
					var record = new object[values.Length];
					var count = values.Length;
					for (var i = 0; i < count; i++)
					{
						if (_types.ContainsKey(aliases[i]))
						{
							record[i] = ConvertTo(_types[aliases[i]], values[i]);
						}
						else
						{
							record[i] = values[i];
						}
					}
					results.Add(transformer.TransformTuple(record, aliases));
				}
				else
				{
					results.Add(transformer.TransformTuple(values, aliases));
				}

				row++;
			}

			reader.Dispose();
			return transformer.TransformResult(results);
		}

		public void WriteAll(IEnumerable data, IDataTransformer transformer)
		{
			if (transformer == null) 
			{
				throw new DataTransformerIsNullException();
			}

			var cellCount = -1;
			var writer = CreateWriter();

            if (_dialect.HasHeader && _headers != null && _headers.Length > 0)
            {
                writer.WriteRow(_headers);
            }

            foreach (var row in data)
            {
                var cells = transformer.TransformRow(row);
                if (cells != null)
                {
                    if (cellCount == -1)
                    {
                        cellCount = cells.Length;
                    }
                    else if (cells.Length != cellCount)
                    {
                        throw new NotEqualCellCountInRowsException(cells.Length, cellCount);
                    }
                }

                writer.WriteRow(cells);
            }

			writer.Dispose();
		}

		protected static object ConvertTo(Type type, string inject)
		{
			if (inject == null || inject.Trim().Length == 0)
			{
				return type.IsValueType ? Activator.CreateInstance(type) : null;
			}

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				var nc = new NullableConverter(type);
				type = nc.UnderlyingType;
			}

			if (type.IsInstanceOfType(inject))
			{
				return inject;
			}
			else if (type == typeof(int))
			{
				int temp;
				if (int.TryParse (inject, out temp)) 
				{
					return temp;
				}

				return null;
			}
			else if (typeof(IConvertible).IsAssignableFrom(type))
			{
				return Convert.ChangeType(inject, type);
			}

			//Maybe we have a constructor that accept the type?
			var ctor = type.GetConstructor(new Type[] { inject.GetType() });
			if (ctor != null)
			{
				return Activator.CreateInstance(type, inject);
			}

			//Maybe we have a Parse method ??
			var parseMethod = type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public);
			if (parseMethod != null)
			{
				return parseMethod.Invoke(null, new object[] { inject });
			}

			throw new CannotConvertValueToRequestType(inject, inject.GetType(), type);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{

				}

				IsDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~CSVAdapter()
		{
			Dispose(false);
		}
	}
}

