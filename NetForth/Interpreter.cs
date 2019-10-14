using System.Collections.Generic;
using System.IO;
using NetForth.WordInterpreters;

namespace NetForth
{
    public class Interpreter
    {
        //internal static readonly Stack<WordInterpreter> InterpreterStack = new Stack<WordInterpreter>();

        private readonly TextReader _sr;
        private readonly Tokenizer _tokenizer;

	    public Interpreter(TextReader sr)
        {
            if ((int)Session.Memory == 0)
            {
                throw new NfException("Interpreting without a valid session");
            }
            _sr = sr;
            _tokenizer = new Tokenizer(_sr);
	    }

        public Interpreter(string str) : this (new StringReader(str)) { }

        public void Interpret()
        {
            DataStack.Stack.Clear();
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

                var ret = evaluable.NewEval(_tokenizer);

				if (ret == Evaluable.ExitType.Exit)
                {
                    DataStack.Stack.Clear();
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
    }
}
