﻿using System.Text;
using static NetForth.Session;

namespace NetForth.WordInterpreters
{
	internal static class DoAction
	{
        internal static readonly ForthStack<int> LoopIndices = new ForthStack<int>();

    	private class DoPrim : Evaluable
		{
			private readonly WordList _wlDo;
			private int _iLoop;
			private readonly bool _plusLoop;
			private readonly bool _isQuestDo;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                
                if (_isQuestDo)
                {
                    sb.Append("?");
                }

                sb.Append("do ");
                sb.Append(_wlDo);
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

            internal override ExitType Eval(Tokenizer tokenizer = null, WordListBuilder _ = null)
            {
                _iLoop = LoopIndices.Count;
                ExitType et;
                var iStart = Stack.Pop();
                var iEnd = Stack.Pop();
                if (_isQuestDo && iStart == iEnd)
                {
                    return ExitType.Okay;
                }

                LoopIndices.Push(iStart);

                // Break into two nearly identical cases for speed
                if (iEnd > iStart)
                {
                    do
                    {
                        et = _wlDo.Eval();
                        if (et != ExitType.Okay)
                        {
                            return et == ExitType.Leave ? ExitType.Okay : et;
                        }

                        if (_plusLoop)
                        {
                            LoopIndices[_iLoop] += Stack.Pop();
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
                        et = _wlDo.Eval();
                        if (et != ExitType.Okay)
                        {
                            return et == ExitType.Leave ? ExitType.Okay : et;
                        }

                        if (_plusLoop)
                        {
                            LoopIndices[_iLoop] += Stack.Pop();
                        }
                        else
                        {
                            --LoopIndices[_iLoop];
                        }
                    } while (LoopIndices[_iLoop] > iEnd);
                }

                LoopIndices.Pop();
                return ExitType.Okay;
            }
        }

        internal static void Do(Tokenizer tokenizer, WordListBuilder wlbParent, bool isQuestDo)
		{
            var _wlbDo = new WordListBuilder();

            while (true)
            {
                var word = tokenizer.NextToken().ToLower();

                if (word == "loop")
                {
                    wlbParent.Add(new DoPrim(_wlbDo.Realize(), false, isQuestDo));
                    return;
                }

                if (word == "+loop")
                {
                    wlbParent.Add(new DoPrim(_wlbDo.Realize(), true, isQuestDo));
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
					evaluable.Eval(tokenizer, _wlbDo);
                    continue;
                }

                _wlbDo.Add(evaluable);
            }

        }

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
    }
}
