using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetForth
{
    internal class WordList : Evaluable
    {
        protected readonly List<Evaluable> _subwords;
		// Is this for a defined word or created on the fly by flow of control constructs?
        protected readonly bool _isDefined;
        protected readonly string _name;

        private bool _leave;

        public WordList(string name, List<Evaluable> subwords, bool isDefined = false)
        {
            _subwords = subwords ?? new List<Evaluable>();
            _isDefined = isDefined;
            _name = name;
        }

        internal override void Leave(ExitType exitType)
        {
            _leave = true;
            if (!_isDefined || exitType == ExitType.Leave)
            {
                _parent.Leave(exitType);
            }
        }

        public WordList(string name, params Evaluable[] subwords) : this(name, subwords.ToList()) { }

        protected override void InnerEval(WordListBuilder _)
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

        public override string ToString()
        {
            var sb = new StringBuilder();
            var prependBlank = false;
            foreach (var evaluable in _subwords)
            {
                var lead = prependBlank ? " " : "";
                prependBlank = true;
                sb.Append(lead + evaluable);
            }

            return sb.ToString();
        }
    }
}
