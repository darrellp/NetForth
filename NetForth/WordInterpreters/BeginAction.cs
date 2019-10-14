namespace NetForth.WordInterpreters
{
    internal static class BeginAction
	{
		private class BeginPrim : Evaluable
		{
			private readonly WordList _wlBegin;
            private readonly bool _isUntil;
            readonly Evaluable _evalCond;

			internal BeginPrim(WordList wlBegin, bool isUntil = false, Evaluable evalCond = null)
			{
				_wlBegin = wlBegin;
                _isUntil = isUntil;
                _evalCond = evalCond;
            }

            internal override ExitType Eval()
            {
                while (true)
                {
                    ExitType et;
                    if (_evalCond != null)
                    {
                        et = _evalCond.Eval();

                        if (et != ExitType.Okay)
                        {
                            return et;
                        }
                        if (DataStack.Stack.Pop() == 0)
                        {
                            return ExitType.Okay;
                        }
                    }
                    et = _wlBegin.Eval();
                    if (et != ExitType.Okay)
                    {
                        return et;
                    }

					if (_isUntil)
                    {
                        if (DataStack.Stack.Pop() != 0)
                        {
                            return ExitType.Okay;
                        }
                    }
                }
            }

            public override string ToString()
            {
                if (_evalCond != null)
                {
                    return $"begin {_evalCond} while {_wlBegin}";
                }

                var terminator = _isUntil ? "until" : "again";

                return $"begin {_wlBegin} {terminator}";
            }
        }

        internal static void Begin(Tokenizer tokenizer, WordListBuilder wlbParent)
        {
		    var wlbBegin = new WordListBuilder();
            Evaluable evalCond = null;
            while (true)
            {
                var word = tokenizer.NextToken().ToLower();

                switch (word)
                {
                    case "again":
                        wlbParent.Add(new BeginPrim(wlbBegin.Realize()));
                        return;

                    case "until":
                        wlbParent.Add(new BeginPrim(wlbBegin.Realize(), true));
                        return;

                    case "while":
                        evalCond = wlbBegin.Realize();
                        wlbBegin.Clear();
                        continue;

                    case "repeat" when evalCond == null:
                        throw new NfException("\"begin...repeat\" with no while");

                    case "repeat":
                        wlbParent.Add(new BeginPrim(wlbBegin.Realize(), false, evalCond));
                        return;
                }

                var evaluable = Interpreter.ParseWord(word);
			    if (evaluable == null)
			    {
				    // TODO: get more robust error handling
				    throw new NfException($"Couldn't locate word {word}");
			    }

			    if (evaluable.IsImmediate)
			    {
				    // Only runs at compile time so no need to supply a parent here.
				    evaluable.Eval(tokenizer, wlbBegin);
				    continue;
			    }
                wlbBegin.Add(evaluable);
            }
		}
	}
}
