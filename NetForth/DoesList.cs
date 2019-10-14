using System.Collections.Generic;

namespace NetForth
{
	internal class DoesList : WordList
    {
        private List<Evaluable> _doeswords;

        public DoesList(string name, List<Evaluable> subwords, List<Evaluable> doeswords, bool isDefined = false) : base(subwords, isDefined)
        {
            _doeswords = doeswords;
        }

        protected virtual void Eval(WordListBuilder _)
        {
        }
    }
}
