using System;
using System.Linq;
using System.Reflection;
using NetForth.WordInterpreters;
using static NetForth.Session;

namespace NetForth.Primitives
{
	#region Compilable
	// Primitives that must compile code and place a resulting primitive in their parent's word list
	// (i.e., but, do and most flow of control words)
    #endregion

	#region LookAhead
	// Primitives that must scan ahead at upcoming words in the stream but do not
	// compile anything into the parent's evolving word list (i.e., Constant, Create, etc.)
    #endregion

	#region ThrowPrimitive
    #endregion

	#region Primitive
    #endregion

	#region DotNet
    internal class DotNetDelegatePrimitive : Evaluable
    {
        private readonly Type _tRet;
        private readonly Type[] _tParms;
        private readonly MethodInfo _method;
        private readonly object _obj;

        internal override ExitType Eval(Tokenizer tokenizer = null, WordListBuilder wlb = null)
        {
            var passedParms = new object[_tParms.Length];
            for (var i = _tParms.Length - 1; i >= 0; i--)
            {
                var val = Stack.Pop();
                if (_tParms[i] == typeof(int))
                {
                    passedParms[i] = val;
                }
                else
                {
                    passedParms[i] = DotNetObjects[val];
                }
            }

            var result = _method.Invoke(_obj, passedParms);
            CallAction.HandleResult(_tRet, result);
            return ExitType.Okay;
        }

        internal DotNetDelegatePrimitive(MethodInfo method, object obj = null)
        {
            _method = method;
            _tRet = _method.ReturnType;
            _tParms = _method.GetParameters().Select(pi => pi.ParameterType).ToArray();
            _obj = obj;
        }
		internal DotNetDelegatePrimitive(Delegate del) : this (del.Method) { }
    }
	#endregion
}
