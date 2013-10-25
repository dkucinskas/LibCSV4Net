using System.Collections;

namespace LibCSV
{
	/// <summary>
	/// Transforms original data into tabular and vice versa.
	/// </summary>
	public interface IDataTransformer
	{
		/// <summary>
		/// Transforms tabular data into custom object.
		/// </summary>
		/// <param name="tuple">Tabular data</param>
		/// <param name="aliases">Column names</param>
		/// <returns>Custom object</returns>
		object TransformTuple(object[] tuple, string[] aliases);
		
		/// <summary>
		/// Transforms and aggregates all tabular data into
		/// collection of custom onjects.
		/// </summary>
		/// <param name="result">Tabular data</param>
		/// <returns>Custom collection</returns>
		IEnumerable TransformResult(IEnumerable result);
		
		/// <summary>
		/// Transforms custom object into tabular data.
		/// </summary>
		/// <param name="tuple">Custom object</param>
		/// <returns>Raw of tabular data</returns>
		object[] TransformRow(object tuple);
	}
}
