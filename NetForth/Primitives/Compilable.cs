using System;

namespace NetForth.Primitives
{
    internal class Compilable : Evaluable
    {
        private readonly Action<Tokenizer, WordListBuilder> _compileAction;

        internal override ExitType Eval(Tokenizer tokenizer = null, WordListBuilder wlb = null)
        {
            _compileAction(tokenizer, wlb);
            return ExitType.Okay;
        }

        internal Compilable(Action<Tokenizer, WordListBuilder> action)
        {
            _compileAction = action;
            IsImmediate = true;
        }
    }
}