using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using LibCSV.Dialects;

namespace LibCSV
{
	/// CSV adapter.
	/// Advanced csv reader that supports read all of records and transformations.
	public class CSVAdapter : IDisposable
	{
		private bool _disposed = false;
		private string _filename = null;
		private string _encoding = null;
		private TextReader _reader = null;
		private TextWriter _writer = null;
		private Dialect _dialect = null;

		private IDictionary<string, Type> _types = new Dictionary<string, Type>();

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
			if (_reader != null)
				return new CSVReader(_dialect, _reader);
			else
				return new CSVReader(_dialect, _filename, _encoding);

		}

		private CSVWriter CreateWriter()
		{
			if (_writer != null)
				return new CSVWriter(_dialect, _writer);
			else
				return new CSVWriter(_dialect, _filename, _encoding);
		}

		public IEnumerable ReadAll(IDataTransformer transformer)
		{
			IList results = new ArrayList();

			CSVReader reader = CreateReader();

			int row = 0;
			string[] aliases = null;

			while (reader.NextRecord())
			{
				string[] values = reader.GetCurrentRecord();

				if (row == 0 && _dialect.HasHeader)
				{
					aliases = values;
				}
				else if (_types.Count > 0)
				{
					object[] record = new object[values.Length];
					int count = values.Length;
					for (int i = 0; i < count; i++)
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
			bool isFirst = true;
			CSVWriter writer = CreateWriter();

			foreach (object row in data)
			{
				if (isFirst && _dialect.HasHeader)
				{
					string[] stringRow = row as string[];

					if (stringRow != null)
					{
						writer.WriteRow(stringRow);
						stringRow = null;
					}

					isFirst = false;
				}
				else
				{
					writer.WriteRow(transformer.TransformRow(row));
				}

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

			if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
			{
				NullableConverter nc = new NullableConverter(type);
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
			ConstructorInfo ctor = type.GetConstructor(new Type[] { inject.GetType() });
			if (ctor != null)
			{
				return Activator.CreateInstance(type, inject);
			}

			//Maybe we have a Parse method ??
			MethodInfo parseMethod = type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public);
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

