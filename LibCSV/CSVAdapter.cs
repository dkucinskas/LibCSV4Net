using System;
using System.Collections;
using System.Collections.Generic;
using LibCSV.Dialects;
using System.IO;
using System.ComponentModel;
using System.Reflection;

namespace LibCSV
{
	/// CSV adapter.
	/// Advanced csv reader that supports read all of records and transformations.
	public class CSVAdapter : CSVReader
	{
		private IDictionary<string, Type> _types = new Dictionary<string, Type>();
		
		public CSVAdapter (Dialect dialect, string filename, string encoding)
			: base(dialect, filename, encoding)
		{
		}
		
		public CSVAdapter (Dialect dialect, TextReader reader)
			: base(dialect, reader)
		{
		}
		
		public void SetColumn(string alias, Type type) 
		{
			_types.Add(alias, type);
		}
		
		public IEnumerable ReadAll(IResultTransformer transformer)
		{
			IList results = new ArrayList();
			
			int row = 0;
			string[] aliases = null;
			
			while (NextRecord())
			{
				string[] values = GetCurrentRecord();
				
				if (row == 0 && Dialect.HasHeader)
				{
					aliases = values;
				}
				else if (_types.Count > 0)
				{
					object[] record = new object[values.Length];
					int count = values.Length;
					for(int i = 0; i < count; i++)
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
			
			return transformer.TransformResult(results);
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
	}
}

