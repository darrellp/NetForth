using System;

namespace NetForth.Primitives
{
    internal class Primitive : Evaluable
    {
        private readonly Func<ExitType> _action;
        internal string Name { get; }

        internal Primitive(Action action, string name, bool isImmediate = false)
        {
            _action = () =>
            {
                action();
                return ExitType.Okay;
            };
            Name = name;
            IsImmediate = isImmediate;
        }

        internal override ExitType Eval(Tokenizer tokenizer = null, WordListBuilder wlb = null)
        {
            _action();
            return ExitType.Okay;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}