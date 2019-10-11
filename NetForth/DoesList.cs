using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetForth
{
	internal class DoesList : WordList
    {
        private List<Evaluable> _doeswords;

        public DoesList(string name, List<Evaluable> subwords, List<Evaluable> doeswords, bool isDefined = false) : base(name, subwords, isDefined)
        {
            _doeswords = doeswords;
        }

        protected override void InnerEval(WordListBuilder _)
        {
            base.InnerEval(_);

        }
    }
}
