using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using LibCSV.Dialects;

namespace LibCSV
{
	/// Advanced csv reader/writer that supports read/write of all records and transformations.
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

            //TODO: fix this //:~
            if (headers == null)
                throw new Exception();

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

			while (reader.NextRecord())
			{
				var values = reader.GetCurrentRecord();

				if (row == 0 && _dialect.HasHeader)
				{
					aliases = values;
				}
				else if (_types.Count > 0)
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
			reader = null;

			return transformer.TransformResult(results);
		}

		public void WriteAll(IEnumerable data, IDataTransformer transformer)
		{
            //TODO: fix this //:~
            if (transformer == null)
                throw new Exception();

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
                        throw new Exception(string.Format(
                            "Cell count in all rows must be equal. Tried insert row with cell count {0}, but " +
                            "previously inserted row or header with cell count {1}.",
                            cells.Length, cellCount));
                    }
                }

                writer.WriteRow(cells);
                cells = null;
            }

			writer.Dispose();
			writer = null;
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
				if (int.TryParse(inject, out temp))
					return temp;
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

			throw new ArgumentException(string.Format(
				"Cannot convert value '{0}' of type '{1}' to request type '{2}'",
				inject,
				inject.GetType(),
				type));
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

