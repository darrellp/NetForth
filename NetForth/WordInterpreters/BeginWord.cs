namespace NetForth.WordInterpreters
{
	internal class BeginWord : WordInterpreter
	{
		private class BeginPrim : Evaluable
		{
			private readonly WordList _wlBegin;
			private bool _leave;

			internal BeginPrim(WordList wlBegin)
			{
				_wlBegin = wlBegin;
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
						_wlBegin.Eval(this);
						if (_leave)
						{
							return;
						}
					}
				}
				finally
				{
					_leave = false;
				}
			}
		}

		private readonly WordListBuilder _wlbParent;
		private readonly WordListBuilder _wlbBegin = new WordListBuilder();

		internal BeginWord(WordListBuilder wlb)
		{
			_wlbParent = wlb;
		}

		internal override void InterpretWord(string word)
		{
			if (word == "again")
			{
				_wlbParent.Add(new BeginPrim(_wlbBegin.Realize()));
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
