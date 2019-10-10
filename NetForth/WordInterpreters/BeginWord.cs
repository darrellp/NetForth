namespace NetForth.WordInterpreters
{
	internal class BeginWord : WordInterpreter
	{
		private class BeginPrim : Evaluable
		{
			private readonly WordList _wlBegin;
			private bool _leave;
            private readonly bool _isUntil;
            Evaluable _evalCond;

			internal BeginPrim(WordList wlBegin, bool isUntil = false, Evaluable evalCond = null)
			{
				_wlBegin = wlBegin;
                _isUntil = isUntil;
                _evalCond = evalCond;
            }

			internal override void Leave(ExitType exitType)
			{
				_leave = true;
				if (exitType == ExitType.Exit)
				{
					_parent?.Leave(exitType);
				}
			}

            protected override void InnerEval(WordListBuilder _)
			{
				try
				{
					while (true)
					{
                        if (_evalCond != null)
                        {
                            _evalCond.Eval();
                            if (DataStack.Stack.Pop() == 0)
                            {
                                return;
                            }
                        }
						_wlBegin.Eval(this);
						if (_leave)
						{
							return;
						}

                        if (_isUntil)
                        {
                            if (DataStack.Stack.Pop() != 0)
                            {
                                return;
                            }
                        }
					}
				}
				finally
				{
					_leave = false;
				}
			}

            public override string ToString()
            {
                if (_evalCond != null)
                {
                    return $"begin {_evalCond.ToString()} while {_wlBegin.ToString()}";
                }

                var terminator = _isUntil ? "until" : "again";

                return $"begin {_wlBegin.ToString()} {terminator}";
            }
        }

		private readonly WordListBuilder _wlbParent;
		private readonly WordListBuilder _wlbBegin = new WordListBuilder();
        private Evaluable _evalCond;

		internal BeginWord(WordListBuilder wlb)
		{
			_wlbParent = wlb;
		}

        internal override void InterpretWord(string word)
        {
            switch (word)
            {
                case "again":
                    _wlbParent.Add(new BeginPrim(_wlbBegin.Realize()));
                    Interpreter.InterpreterStack.Pop();
                    return;

                case "until":
                    _wlbParent.Add(new BeginPrim(_wlbBegin.Realize(), true));
                    Interpreter.InterpreterStack.Pop();
                    return;

                case "while":
                    _evalCond = _wlbBegin.Realize();
                    _wlbBegin.Clear();
                    return;

                case "repeat" when _evalCond == null:
                    Interpreter.InterpreterStack.Pop();
                    throw new NfException("\"begin...repeat\" with no while");

                case "repeat":
                    _wlbParent.Add(new BeginPrim(_wlbBegin.Realize(), false, _evalCond));
                    Interpreter.InterpreterStack.Pop();
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
				evaluable.Eval(null, _wlbBegin);
				return;
			}

			_wlbBegin.Add(evaluable);
		}
	}
}
