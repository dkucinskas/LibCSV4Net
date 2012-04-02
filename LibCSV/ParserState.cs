
namespace LibCSV
{
	public enum ParserState
	{
		StartOfRecord,
		StartOfField,
		EscapedCharacter,
		InField,
		InQuotedField,
		EscapeInQuotedField,
		QuoteInQuotedField,
		EndOfRecord
	}
}

