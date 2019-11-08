using System;
using System.Text;
using NetForth.Primitives;
using static NetForth.Session;

namespace NetForth.WordInterpreters
{
    enum StringType
    {
        Forth,
        DotNet,
        Type,
        Stack
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
                    var pCountedString = Memory.Allocate(text.Length * sizeof(char) + Session.StringLengthSize);
                    Memory.StoreCString(pCountedString, text);
                    finalValue = pCountedString;
                    break;

				case StringType.DotNet:
                    var index = SaveManagedObject(text);
                    finalValue = index;
                    break;

				case StringType.Type:
                    var type = Type.GetType(text);
                    if (type == null)
                    {
                        throw new NfException($"Unrecognized .NET type in t\": {text}");
                    }

                    if (type.IsGenericType)
                    {
                        var genericParameterCount = type.GetGenericArguments().Length;
                        var parmTypes = CallAction.GetParmTypes($"Invalid types for generic type {text}", genericParameterCount);
                        type = type.MakeGenericType(parmTypes);
                    }
                    finalValue = SaveManagedObject(type);
					break;

				case StringType.Stack:
                    var pStackString = Memory.AllocateStackString(text.Length);
                    Memory.StoreString(pStackString, text);
                    finalValue = pStackString;
                    break;
			}
			if (wlbParent == null)
            {
                Stack.Push(finalValue);
                if (st == StringType.Stack)
                {
                    Stack.Push(text.Length);
                }
            }
            else
            {
                wlbParent.Add(new IntPrim(finalValue));
                if (st == StringType.Stack)
                {
                    wlbParent.Add(new IntPrim(text.Length));
                }
            }
		}
	}
}
