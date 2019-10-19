using System;
using System.Text;
using static NetForth.Session;

namespace NetForth.WordInterpreters
{
    enum StringType
    {
        Forth,
        DotNet,
        Type
    }
	static class FStringAction
	{
        internal static void FString(Tokenizer tokenizer, WordListBuilder wlbParent, StringType st = StringType.Forth)
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
            }

            var text = sbText.ToString();
            int finalValue = -1;
            switch (st)
            {
				case StringType.Forth:
                    var pCountedString = Memory.Allocate(text.Length + Session.StringLengthSize);
                    Memory.StoreCString(pCountedString, text);
                    finalValue = pCountedString;
                    break;

				case StringType.DotNet:
                    var index = SaveManagedObject(text);
                    finalValue = index;
                    break;

				case StringType.Type:
                    finalValue = SaveManagedObject(Type.GetType(text));
					break;
			}
            if (wlbParent == null)
            {
                Stack.Push(finalValue);
            }
            else
            {
                wlbParent.Add(new IntPrim(finalValue));
            }
		}
	}
}
