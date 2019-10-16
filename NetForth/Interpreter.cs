using System.IO;
using static NetForth.Session;

namespace NetForth
{
    public class Interpreter
    {
		#region Private variables
		private readonly Tokenizer _tokenizer;
		#endregion

		#region Constructor
		public Interpreter(TextReader sr)
        {
            if ((int)Session.ForthMemory == 0)
            {
                throw new NfException("Interpreting without a valid session");
            }

            _tokenizer = new Tokenizer(sr);
	    }
		#endregion

		#region Interpreter
		public Interpreter(string str) : this (new StringReader(str)) { }

        public void Interpret()
        {
            Stack.Clear();
            while (true)
            {
                var word = _tokenizer.NextToken(true);
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

                var ret = evaluable.Eval(_tokenizer);

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
