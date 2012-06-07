
namespace LibCSV
{
	public struct StyleDesc
	{
		public QuoteStyle Style;
		public string Name;

		public StyleDesc(QuoteStyle style, string name)
		{
			this.Style = style;
			this.Name = name;
		}
	}
}

