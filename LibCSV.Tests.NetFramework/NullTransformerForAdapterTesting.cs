using System.Collections;
using LibCSV;
using NUnit.Framework;

namespace LibCSV.Tests
{
	internal class NullTransformerForAdapterTesting : IDataTransformer
	{
		private string[] _aliases;
		private IList _records;
		private int _row;
		
		public NullTransformerForAdapterTesting(string[] expectedAliases, IList expectedResults)
		{
			_aliases = expectedAliases;
			_records = expectedResults;
		}
		
		public object TransformTuple(string[] tuple, string[] aliases)
		{
			if (_aliases != null)
			{
				Assert.AreEqual(_aliases, aliases);
			}
			
			if (_records != null)
			{
				Assert.AreEqual(_records[_row], tuple);
			}
			
			_row++;
			return null;
		}
		
		public IEnumerable TransformResult(IEnumerable result)
		{
			return null;
		}
		
		public object[] TransformRow(object tuple)
		{
			if (_records != null)
			{
				Assert.AreEqual(_records[_row], tuple);
				_row++;
			}
			
			return (object[])tuple;
		}
	}
}
