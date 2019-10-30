using System;
using System.Reflection;
using NetForth.WordInterpreters;

namespace NetForth.Primitives
{
    internal class ConstructorPrimitive : DotNetPrimitive
    {
        private readonly ConstructorInfo _constructor;

        internal ConstructorPrimitive(ConstructorInfo constructor, Type[] parmTypes) : base(constructor.DeclaringType, parmTypes)
        {
            _constructor = constructor;
        }

        internal override ExitType Eval(Tokenizer tokenizer = null, WordListBuilder wlb = null)
        {
            var passedParms = GetPassedParms();
            var result = _constructor.Invoke(passedParms);
            CallAction.HandleResult(TRet, result);
            return ExitType.Okay;
        }
    }
}