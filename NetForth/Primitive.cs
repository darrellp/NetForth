using System;
using System.Linq;
using System.Reflection;
using NetForth.WordInterpreters;
using static NetForth.Session;

namespace NetForth
{
	#region Compilable
	// Primitives that must compile code and place a resulting primitive in their parent's word list
	// (i.e., but, do and most flow of control words)
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
	#endregion

	#region LookAhead
	// Primitives that must scan ahead at upcoming words in the stream but do not
	// compile anything into the parent's evolving word list (i.e., Constant, Create, etc.)
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
	#endregion

	#region ThrowPrimitive
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
	#endregion

	#region Primitive
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
	#endregion

	#region DotNet
    internal class DotNetPrimitive : Evaluable
    {
        private readonly Type _tRet;
        private readonly Type[] _tParms;
        private readonly MethodInfo _method;
        private readonly Object _obj;

        internal override ExitType Eval(Tokenizer tokenizer = null, WordListBuilder wlb = null)
        {
            var passedParms = new object[_tParms.Length];
            for (var i = _tParms.Length - 1; i >= 0; i--)
            {
                if (_tParms[i] == typeof(int))
                {
                    passedParms[i] = Stack.Pop();
                }
                else
                {
                    passedParms[i] = DotNetObjects[Stack.Pop()];
                }
            }

            var result = _method.Invoke(_obj, passedParms);
            CallAction.HandleResult(_tRet, result);
            return ExitType.Okay;
        }

        internal DotNetPrimitive(MethodInfo method)
        {
            _method = method;
            _tRet = _method.ReturnType;
            _tParms = _method.GetParameters().Select(pi => pi.ParameterType).ToArray();
        }
		internal DotNetPrimitive(Delegate del) : this (del.Method) { }
	}
	#endregion
}
