using System.Collections;

namespace LibCSV
{
	public interface IDataTransformer
	{
		object TransformTuple(object[] tuple, string[] aliases);
		IEnumerable TransformResult(IEnumerable result);

		string[] TransformRow(object tuple);
	}
}
