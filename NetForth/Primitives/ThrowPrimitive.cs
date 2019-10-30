using System;

namespace NetForth.Primitives
{
    internal class ThrowPrimitive : Evaluable
    {
        private readonly Func<ExitType> _action;
        private string Name { get; }

        internal ThrowPrimitive(Func<ExitType> action, string name, bool isImmediate = false)
        {
            _action = action;
            IsImmediate = isImmediate;
            Name = name;
        }

        internal override ExitType Eval(Tokenizer _ = null, WordListBuilder wlb = null)
        {
            return _action();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}