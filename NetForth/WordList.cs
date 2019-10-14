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

        public WordList(string name, List<Evaluable> subwords, bool isDefined = false)
        {
            _subwords = subwords ?? new List<Evaluable>();
            _isDefined = isDefined;
            _name = name;
        }

        protected virtual void Eval(WordListBuilder wlb)
        {
            throw new System.NotImplementedException();
        }

        public WordList(string name, params Evaluable[] subwords) : this(name, subwords.ToList()) { }

        internal override ExitType NewEval(Tokenizer _ = null)
        {
            foreach (var evaluable in _subwords)
            {
                var et = evaluable.NewEval();

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
