using System.Collections.Generic;
using System.Linq;

namespace NetForth
{
    internal class WordList : Evaluable
    {
        private readonly List<Evaluable> _subwords;

        private bool _leave;

        public WordList(List<Evaluable> subwords = null)
        {
            _subwords = subwords ?? new List<Evaluable>();
        }

        internal override void Leave(ExitType exitType)
        {
            _leave = true;
            if (exitType == ExitType.Leave)
            {
                _parent.Leave(exitType);
            }
        }

        public WordList(params Evaluable[] subwords) : this(subwords.ToList()) { }

        internal override void InnerEval(WordListBuilder _)
        {
            try
            {
                foreach (var evaluable in _subwords)
                {
                    evaluable.Eval(this);
                    if (_leave)
                    {
                        return;
                    }
                }
            }
            finally
            {
                _leave = false;
            }
        }
    }
}
