using System.Text;
using static NetForth.DataStack;

namespace NetForth.WordInterpreters
{
    internal class IfWord : WordInterpreter
    {
        private class IfPrim : Evaluable
        {
            private readonly WordList _wlThen;
            private readonly WordList _wlElse;

            internal IfPrim(WordList wlThen, WordList wlElse)
            {
                _wlThen = wlThen;
                _wlElse = wlElse;
            }

            protected override void InnerEval(WordListBuilder wlb)
            {
                if (Stack.Pop() != 0)
                {
                    _wlThen.Eval(this);
                }
                else
                {
                    _wlElse?.Eval(this);
                }
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append("if ");
                if (_wlElse != null)
                {
                    sb.Append(_wlElse.ToString() + " else ");
                }
                sb.Append(_wlThen.ToString() + " then");
                return sb.ToString();
            }
        }

        private readonly WordListBuilder _wlbParent;
        private WordListBuilder _wlbThenClause = new WordListBuilder();
        private WordListBuilder _wlbElseClause = new WordListBuilder();

		internal IfWord(WordListBuilder wlb)
        {
            _wlbParent = wlb;
        }

		internal override void InterpretWord(string word)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (word)
            {
                case "then":
                    _wlbParent.Add(new IfPrim(_wlbThenClause.Realize(), _wlbElseClause?.Realize()));
                    Interpreter.InterpreterStack.Pop();
                    return;

                case "else":
                    _wlbElseClause = _wlbThenClause;
                    _wlbThenClause = new WordListBuilder();
                    return;
            }

            var evaluable = EvalWord.ParseWord(word);
            if (evaluable == null)
            {
                // TODO: get more robust error handling
                throw new NfException($"Couldn't locate word {word}");
            }

            if (evaluable.IsImmediate)
            {
                // Only runs at compile time so no need to supply a parent here.
                evaluable.Eval(null, _wlbThenClause);
                return;
            }

            _wlbThenClause.Add(evaluable);
        }
	}
}
