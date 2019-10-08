using System.Text;

namespace NetForth.WordInterpreters
{
	class FString : WordInterpreter
	{
        readonly StringBuilder sbText = new StringBuilder();
        readonly WordListBuilder _wlbParent;
		bool _prependSpace;

		internal FString(WordListBuilder wlb)
		{
			_wlbParent = wlb;
		}

		internal override void InterpretWord(string word)
		{
			var done = false;
			if (word.EndsWith("\""))
			{
				done = true;
				word = word.Substring(0, word.Length - 1);
			}
			if (_prependSpace)
			{
				sbText.Append(" ");
			}
			_prependSpace = true;
			sbText.Append(word);

			if (done)
			{
				var text = sbText.ToString();
				var pCountedString = Memory.Allocate(text.Length + sizeof(int));
				Memory.StoreCString(pCountedString, text);
				_wlbParent.Add(new IntPrim(pCountedString));
				Interpreter.InterpreterStack.Pop();
			}
		}
	}
}
