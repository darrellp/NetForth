namespace NetForth.WordInterpreters
{
    internal class EvalWord : WordInterpreter
	{
		internal override void InterpretWord(string word)
        {
            if (word == "\n")
            {
                return;
            }
            var evaluable = ParseWord(word);
            if (evaluable == null)
            {
                // TODO: get more robust error handling
                throw new NfException($"Couldn't locate word {word}");
            }
            evaluable.Eval();
        }

        internal static Evaluable ParseWord(string word)
        {
			var lookup = Vocabulary.Lookup(word);
			if (lookup == null)
			{
				var isNumber = int.TryParse(word, out int val);
				return isNumber ? new IntPrim(val) : null;
			}
            return lookup;
        }
	}
}
