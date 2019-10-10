using System.Text;
using static NetForth.DataStack;

namespace NetForth.WordInterpreters
{
	internal class DoWord : WordInterpreter
	{
		internal static readonly ForthStack<int> LoopIndices = new ForthStack<int>();

		private class DoPrim : Evaluable
		{
			private readonly WordList _wlDo;
			private int _iLoop;
			private readonly bool _plusLoop;
			private readonly bool _isQuestDo;
            private bool _leave;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                
                if (_isQuestDo)
                {
                    sb.Append("?");
                }

                sb.Append("do ");
                sb.Append(_wlDo.ToString());
                sb.Append(" ");

                if (_plusLoop)
                {
                    sb.Append("+");
                }

                sb.Append("loop");
                return sb.ToString();
            }

            internal DoPrim(WordList wlDo, bool plusLoop = false, bool isQuestDo = false)
            {
                _wlDo = wlDo;
				_plusLoop = plusLoop;
				_isQuestDo = isQuestDo;
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
				_iLoop = LoopIndices.Count;

                var iStart = Stack.Pop();
                var iEnd = Stack.Pop();
				if (_isQuestDo && iStart == iEnd)
				{
					return;
				}

                LoopIndices.Push(iStart);

				// Break into two nearly identical cases for speed
                try
                {
                    if (iEnd > iStart)
                    {
                        do
                        {
                            _wlDo.Eval(this);
                            if (_leave)
                            {
                                return;
                            }

                            if (_plusLoop)
                            {
                                LoopIndices[_iLoop] += DataStack.Stack.Pop();
                            }
                            else
                            {
                                ++LoopIndices[_iLoop];
                            }
                        } while (LoopIndices[_iLoop] < iEnd);
                    }
                    else
                    {
                        do
                        {
                            _wlDo.Eval(this);
                            if (_leave)
                            {
                                return;
                            }

                            if (_plusLoop)
                            {
                                LoopIndices[_iLoop] += DataStack.Stack.Pop();
                            }
                            else
                            {
                                --LoopIndices[_iLoop];
                            }
                        } while (LoopIndices[_iLoop] > iEnd);
                    }
                }
                finally
                {
                    _leave = false;
                }
				LoopIndices.Pop();
			}
		}

		private readonly WordListBuilder _wlbParent;
		private readonly WordListBuilder _wlbDo = new WordListBuilder();
		private readonly bool _isQuestDo;

        // ReSharper disable InconsistentNaming
        internal static int i()
        {
			return LoopIndices[-1];
		}

		internal static int j()
		{
			return LoopIndices[-2];
		}
        // ReSharper restore InconsistentNaming

		internal DoWord(WordListBuilder wlb, bool isQuestDo = false)
		{
			_wlbParent = wlb;
			_isQuestDo = isQuestDo;
		}

		internal override void InterpretWord(string word)
		{
            if (word == "loop")
            {
                _wlbParent.Add(new DoPrim(_wlbDo.Realize(), false, _isQuestDo));
                Interpreter.InterpreterStack.Pop();
                return;
            }

			if (word == "+loop")
			{
				_wlbParent.Add(new DoPrim(_wlbDo.Realize(), true, _isQuestDo));
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
				evaluable.Eval(null, _wlbDo);
				return;
			}

			_wlbDo.Add(evaluable);
		}
	}
}
