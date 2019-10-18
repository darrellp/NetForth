using System.IO;
using static NetForth.Session;

namespace NetForth
{
    public class Interpreter
    {
		#region Constructor
		public Interpreter()
        {
            if ((int)Session.ForthMemory == 0)
            {
                throw new NfException("Interpreting without a valid session");
            }
	    }
		#endregion

		#region Interpreter
        public void Interpret(string str)
        {
            Interpret(new StringReader(str));
        }

        public void Interpret(TextReader sr, bool clearStack = true)
        {
            if (clearStack)
            {
                Stack.Clear();
            }
            var tokenizer = new Tokenizer(sr);
            while (true)
            {
                var word = tokenizer.NextToken(true);
                if (word == null)
                {
                    return;
                }

                var evaluable = ParseWord(word);
                if (evaluable == null)
                {
                    // TODO: get more robust error handling
                    throw new NfException($"Couldn't locate word {word}");
                }

                var ret = evaluable.Eval(tokenizer);

				if (ret == Evaluable.ExitType.Exit)
                {
                    Stack.Clear();
                    break;
                }
            }
		}

        internal static Evaluable ParseWord(string word)
        {
            var lookup = Vocabulary.Lookup(word);
            if (lookup == null)
            {
                var isNumber = int.TryParse(word, out int val);
                return isNumber ? new IntPrim(val) : null;
            }
            return lookup;
        }
		#endregion
	}
}
