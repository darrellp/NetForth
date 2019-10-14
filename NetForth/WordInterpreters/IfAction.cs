using System.Text;
using static NetForth.DataStack;

namespace NetForth.WordInterpreters
{
    internal static class IfAction
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

            internal override ExitType Eval(Tokenizer _)
            {
                if (Stack.Pop() != 0)
                {
                    var retThen = _wlThen.Eval();

					if (retThen != ExitType.Okay)
                    {
                        return retThen;
                    }
                }
                else
                {
                    if (_wlElse != null)
                    {
                        var retElse = _wlElse.Eval();

                        if (retElse != ExitType.Okay)
                        {
                            return retElse;
                        }
                    }
				}

                return ExitType.Okay;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append("if ");
                if (_wlElse != null)
                {
                    sb.Append(_wlElse + " else ");
                }
                sb.Append(_wlThen + " then");
                return sb.ToString();
            }
        }

        internal static void If(Tokenizer tokenizer, WordListBuilder wlbParent)
        {
            var wlbThenClause = new WordListBuilder();
            var wlbElseClause = (WordListBuilder)null;
            while (true)
            {
                var word = tokenizer.NextToken().ToLower();

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (word)
                {
                    case "then":
                        wlbParent.Add(new IfPrim(wlbThenClause.Realize(), wlbElseClause?.Realize()));
                        return;

                    case "else":
                        wlbElseClause = wlbThenClause;
                        wlbThenClause = new WordListBuilder();
                        continue;
                }

                var evaluable = Interpreter.ParseWord(word);
                if (evaluable == null)
                {
                    // TODO: get more robust error handling
                    throw new NfException($"Couldn't locate word {word}");
                }

                if (evaluable.IsImmediate)
                {
                    evaluable.Eval(tokenizer, wlbThenClause);
                    continue;
                }

                wlbThenClause.Add(evaluable);
            }
		}
    }
}
