using System.Collections.Generic;
using System.IO;
using NetForth.WordInterpreters;

namespace NetForth
{
    public class Interpreter
    {
        internal static readonly Stack<WordInterpreter> InterpreterStack = new Stack<WordInterpreter>();

        private readonly TextReader _sr;

	    public Interpreter(TextReader sr)
        {
            if ((int)Session.Memory == 0)
            {
                throw new NfException("Interpreting without a valid session");
            }
            _sr = sr;
            InterpreterStack.Push(new EvalWord());
	    }

        public Interpreter(string str) : this (new StringReader(str)) { }

        public bool InterpretLine()
        {
            var thisLine = _sr.ReadLine();
            if (thisLine == null)
            {
                return false;
            }
            thisLine = thisLine.Trim();

            foreach (var wordIter in thisLine.Split("\t ".ToCharArray()))
            {
                var word = wordIter.Trim();
                if (word.Length == 0)
                {
                    continue;
                }
                InterpreterStack.Peek().InterpretWord(word);
            }
            InterpreterStack.Peek().InterpretWord("\n");
            return true;
        }

        public void InterpretAll()
        {
            while (InterpretLine()) { }
        }
    }
}
