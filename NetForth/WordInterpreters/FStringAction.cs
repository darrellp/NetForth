using System.Text;
using static NetForth.Session;

namespace NetForth.WordInterpreters
{
	static class FStringAction
	{
        internal static void FString(Tokenizer tokenizer, WordListBuilder wlbParent)
        {
            var sbText = new StringBuilder();
		    var prependSpace = false;

            var done = false;
            // TODO: read tokenizer char by char to properly emulate white space
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
                    var pCountedString = Memory.Allocate(text.Length + Session.StringLengthSize);
                    Memory.StoreCString(pCountedString, text);
                    if (wlbParent == null)
                    {
                        Stack.Push(pCountedString);
                    }
                    else
                    {
                        wlbParent.Add(new IntPrim(pCountedString));
                    }
                }
            }
		}
	}
}
