using System.Collections.Generic;
using System.Linq;

namespace NetForth
{
    internal class WordList : Evaluable
    {
        private readonly List<Evaluable> _subwords;
        // Is this for a defined word or created on the fly by flow of control constructs?
        private readonly bool _isDefined;

        private bool _leave;

        public WordList(List<Evaluable> subwords = null, bool isDefined = false)
        {
            _subwords = subwords ?? new List<Evaluable>();
            _isDefined = isDefined;
        }

        internal override void Leave(ExitType exitType)
        {
            _leave = true;
            if (!_isDefined || exitType == ExitType.Leave)
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
