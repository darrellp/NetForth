namespace NetForth.WordInterpreters
{
    internal class DefinitionWord : WordInterpreter
    {
        private readonly WordListBuilder _wlb = new WordListBuilder();
        private string _currentDefWord;

        internal override void InterpretWord(string word)
        {
            if (_currentDefWord == null)
            {
                _currentDefWord = word;
                return;
            }

            if (word == ";")
            {
                Vocabulary.CurrentVocabulary.AddDefinition(_currentDefWord, _wlb.Realize(true));
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
