using System;
using System.Reflection;
using NetForth.WordInterpreters;

namespace NetForth.Primitives
{
    internal class MethodPrimitive : DotNetPrimitive
    {
        private readonly MethodInfo _method;
        private readonly bool _isStatic;

        internal MethodPrimitive(MethodInfo method, Type[] parmTypes, bool isStatic = false) : base(method.ReturnType, parmTypes)
        {
            _method = method;
            _isStatic = isStatic;
        }

        internal override ExitType Eval(Tokenizer tokenizer = null, WordListBuilder wlb = null)
        {
            var obj = _isStatic ? null : Session.GetManagedObject(Session.Stack.Pop());
            var passedParms = GetPassedParms();
            var result = _method.Invoke(obj, passedParms);
            CallAction.HandleResult(TRet, result);
            return ExitType.Okay;
        }
    }
}