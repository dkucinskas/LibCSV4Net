using System;

namespace LibCSV
{
	public struct StyleDesc
	{
		public QuoteStyle style;
		public string name;
		
		public StyleDesc(QuoteStyle style, string name)
		{
			this.style = style;
			this.name = name;
		}
	}
}

