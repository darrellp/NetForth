using System;

namespace NetForth.Primitives
{
    internal class LookAhead : Evaluable
    {
        private readonly Action<Tokenizer> _action;
        internal string Name { get; }

        internal LookAhead(Action<Tokenizer> action, string name, bool isImmediate = false)
        {
            _action = action;
            IsImmediate = isImmediate;
            Name = name;
        }

        internal override ExitType Eval(Tokenizer tokenizer = null, WordListBuilder _ = null)
        {
            _action(tokenizer);
            return ExitType.Okay;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}