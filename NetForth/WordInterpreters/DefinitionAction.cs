using System.Collections.Generic;

namespace NetForth.WordInterpreters
{
    internal static class DefinitionAction
    {
        private class DoesWord : Evaluable
        {
            private readonly Evaluable _compile;
            private readonly Evaluable _run;

            internal DoesWord(Evaluable compile, Evaluable run)
            {
                _compile = compile;
                _run = run;
            }

            internal override ExitType Eval(Tokenizer tokenizer = null, WordListBuilder wlb = null)
            {
                Session.LastDefinedWord = null;
                _compile.Eval(tokenizer, wlb);
                if (Session.LastDefinedWord != null)
                {
                    var intPrim = Vocabulary.Lookup(Session.LastDefinedWord) as IntPrim;
                    if (intPrim == null)
                    {
                        throw new NfException("LastDefinedWord was not defined as an IntPrim");
                    }

                    List<Evaluable> newDefWords = new List<Evaluable>() {intPrim, _run};
                    Vocabulary.CurrentVocabulary.AddDefinition(Session.LastDefinedWord, new WordList(newDefWords));
                    Session.LastDefinedWord = null;
                }
                return ExitType.Okay;
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
