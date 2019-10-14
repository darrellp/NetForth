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

        public WordList(List<Evaluable> subwords, bool isDefined = false)
        {
            _subwords = subwords ?? new List<Evaluable>();
            _isDefined = isDefined;
        }

        public WordList(string name, params Evaluable[] subwords) : this(subwords.ToList()) { }

        internal override ExitType Eval(Tokenizer _)
        {
            foreach (var evaluable in _subwords)
            {
                var et = evaluable.Eval();

				if (et != ExitType.Okay)
                {
                    if (_isDefined && et == ExitType.Exit)
                    {
                        return ExitType.Okay;
                    }
                    return et;
                }
            }

            return ExitType.Okay;
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
