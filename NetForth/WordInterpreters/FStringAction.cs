using System.Text;

namespace NetForth.WordInterpreters
{
	static class FStringAction
	{
        internal static void FString(Tokenizer tokenizer, WordListBuilder wlbParent)
        {
            var sbText = new StringBuilder();
		    var prependSpace = false;

            var done = false;
            while (!done)
            {
                var word = tokenizer.NextToken();

                if (word.EndsWith("\""))
                {
                    done = true;
                    word = word.Substring(0, word.Length - 1);
                }
                if (prependSpace)
                {
                    sbText.Append(" ");
                }
                prependSpace = true;
                sbText.Append(word);

                if (done)
                {
                    var text = sbText.ToString();
                    var pCountedString = Memory.Allocate(text.Length + sizeof(int));
                    Memory.StoreCString(pCountedString, text);
                    wlbParent.Add(new IntPrim(pCountedString));
                }
            }
		}
	}
}
