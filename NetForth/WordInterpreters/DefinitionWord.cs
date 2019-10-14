using System.Collections.Generic;

namespace NetForth.WordInterpreters
{
    internal static class Definition
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

            protected virtual void Eval(WordListBuilder wlb)
            {
                Session.LastDefinedWord = null;
                if (Session.LastDefinedWord == null)
                {
                    return;
                }

                var oldDefinition = Vocabulary.Lookup(Session.LastDefinedWord);
                var newDefinition = new WordList(Session.LastDefinedWord,
                    new List<Evaluable>() { oldDefinition, _run }, true);
                Vocabulary.CurrentVocabulary.AddDefinition(Session.LastDefinedWord, newDefinition);
            }
        }

        internal static void ParseDefinition(Tokenizer tokenizer)
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
                    evaluable.NewEval(tokenizer, wlb);
                    continue;
                }

                wlb.Add(evaluable);
			}
		}
    }
}
