using System;
using System.Collections;

namespace LibCSV
{
	public interface IResultTransformer
	{
		object TransformTuple(object[] tuple, string[] aliases);
		IEnumerable TransformResult(IEnumerable result);
	}
}
