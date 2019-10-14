namespace NetForth.WordInterpreters
{
    internal static class DefinitionAction
    {
        private class DoesWord : Evaluable
        {
            private Evaluable _compile;
            private Evaluable _run;

            internal DoesWord(Evaluable compile, Evaluable run)
            {
                _compile = compile;
                _run = run;
            }
        }

        internal static void Definition(Tokenizer tokenizer)
        {
            var wlb = new WordListBuilder();
            string currentDefWord = tokenizer.NextToken().ToLower();
            Evaluable execute = null;

            while (true)
            {
				var word = tokenizer.NextToken().ToLower();

                if (word == "does>")
                {
                    execute = wlb.Realize(true, currentDefWord);
                    wlb.Clear();
                    continue;
                }

                if (word == ";")
                {
                    if (execute == null)
                    {
                        Vocabulary.CurrentVocabulary.AddDefinition(currentDefWord, wlb.Realize(true, currentDefWord));
                    }
                    else
                    {
                        var run = wlb.Realize(true, currentDefWord);
                        var compile = execute;
                        Vocabulary.CurrentVocabulary.AddDefinition(currentDefWord, new DoesWord(compile, run));
                    }

                    Session.LastDefinedWord = null;
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
                    evaluable.Eval(tokenizer, wlb);
                    continue;
                }

                wlb.Add(evaluable);
			}
		}
    }
}
