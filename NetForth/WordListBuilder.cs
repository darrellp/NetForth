using System.Collections.Generic;

namespace NetForth
{
    internal class WordListBuilder
	{
        readonly List<Evaluable> _words = new List<Evaluable>();

        internal void Add(Evaluable evaluable)
        {
            _words.Add(evaluable);
        }

        internal WordList Realize(bool isDefined = false)
        {
			return new WordList(_words, isDefined);
        }
	}
}
