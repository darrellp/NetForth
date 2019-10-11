using System.Collections.Generic;

namespace NetForth.WordInterpreters
{
    internal class DefinitionWord : WordInterpreter
    {
        private readonly WordListBuilder _wlb = new WordListBuilder();
        private string _currentDefWord;
        private Evaluable _execute;

        private class DoesWord : Evaluable
        {
            private Evaluable _compile;
            private Evaluable _run;

            internal DoesWord(Evaluable compile, Evaluable run)
            {
                _compile = compile;
                _run = run;
            }

            protected override void InnerEval(WordListBuilder wlb)
            {
                Session.LastDefinedWord = null;
                _compile.Eval();
                if (Session.LastDefinedWord == null)
                {
                    return;
                }

                var oldDefinition = Vocabulary.Lookup(Session.LastDefinedWord);
                var newDefinition = new WordList(Session.LastDefinedWord,
                    new List<Evaluable>() {oldDefinition, _run}, true);
                Vocabulary.CurrentVocabulary.AddDefinition(Session.LastDefinedWord, newDefinition);
            }
        }

        internal override void InterpretWord(string word)
        {
            if (_currentDefWord == null)
            {
                _currentDefWord = word;
                return;
            }

            if (word == "does>")
            {
                _execute = _wlb.Realize(true, _currentDefWord);
                _wlb.Clear();
                return;
            }

            if (word == ";")
            {
                if (_execute == null)
                {
                    Vocabulary.CurrentVocabulary.AddDefinition(_currentDefWord, _wlb.Realize(true, _currentDefWord));
                }
                else
                {
                    var run = _wlb.Realize(true, _currentDefWord);
                    var compile = _execute;
                    Vocabulary.CurrentVocabulary.AddDefinition(_currentDefWord, new DoesWord(compile, run));
				}

                Session.LastDefinedWord = null;
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
                evaluable.Eval(null, _wlb);
                return;
            }

            _wlb.Add(evaluable);
        }
	}
}
