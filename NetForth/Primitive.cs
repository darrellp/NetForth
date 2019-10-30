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
    internal class DotNetPrimitiveNew : Evaluable
	{
        protected Type TRet;
        protected Type[] TParms;

        protected DotNetPrimitiveNew(Type tRet, Type[] tParms)
        {
            TRet = tRet;
            TParms = tParms;
        }
        protected DotNetPrimitiveNew() { }

		protected object[] GetPassedParms()
        {
            var passedParms = new object[TParms.Length];
            for (var i = TParms.Length - 1; i >= 0; i--)
            {
                var val = Stack.Pop();
                passedParms[i] = TParms[i] == typeof(int) ? val : DotNetObjects[val];
            }

            return passedParms;
        }
    }

	internal class MethodPrimitive : DotNetPrimitiveNew
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
            var obj = _isStatic ? null : GetManagedObject(Stack.Pop());
            var passedParms = GetPassedParms();
            var result = _method.Invoke(obj, passedParms);
            CallAction.HandleResult(TRet, result);
            return ExitType.Okay;
        }
    }

    internal class ConstructorPrimitive : DotNetPrimitiveNew
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

    internal class IndexerPrimitive : DotNetPrimitiveNew
    {
        private readonly PropertyInfo _indexerProperty;

        internal IndexerPrimitive(Type objType, Type[] parmTypes)
        {
            TParms = parmTypes;
            (TRet, _indexerProperty) = IndexerReturnType(objType, parmTypes);
            if (objType.IsArray)
            {
                // We don't access actual arrays with an indexer property
                return;
            }

            // Locate the proper indexer property
            foreach (var property in objType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var indexParameters = property.GetIndexParameters();
                if (indexParameters.Length > 0)
                {
                    if (indexParameters.Select(ip => ip.ParameterType).Zip(parmTypes, (t1, t2) => t1 == t2).All(t => t))
                    {
                        _indexerProperty = property;
                    }
                }
            }

            if (_indexerProperty == null)
            {
                throw new NfException($"Invalid parameters to index into {objType}");
            }
        }

		internal override ExitType Eval(Tokenizer tokenizer = null, WordListBuilder wlb = null)
        {
            var obj = GetManagedObject(Stack.Pop());
            var passedParms = GetPassedParms();
            var result = IndexerEval(obj, passedParms);
            CallAction.HandleResult(TRet, result);
            return ExitType.Okay;
        }

        private object IndexerEval(object obj, object[] passedParms)
        {
            var objType = obj.GetType();

            if (objType.IsArray)
            {
                var intParms = passedParms.Cast<int>().ToArray();
                return ((Array)obj).GetValue(intParms);
            }

            return _indexerProperty.GetValue(obj, passedParms);

		}

		private static (Type, PropertyInfo) IndexerReturnType(Type objType, Type[] parmTypes)
        {
            if (objType.IsArray)
            {
                return (objType.GetElementType(), null);
            }

            foreach (var property in objType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var indexParameters = property.GetIndexParameters();
                if (indexParameters.Length > 0)
                {
                    if (indexParameters.Select(ip => ip.ParameterType).Zip(parmTypes, (t1, t2) => t1 == t2).All(t => t))
                    {
                        return (property.PropertyType, property);
                    }
                }
            }

            throw new NfException("No indexer matches signature");
        }
	}

	internal class DotNetPrimitive : Evaluable
    {
        private readonly Type _tRet;
        private readonly Type[] _tParms;
        private readonly MethodInfo _method;
        private readonly ConstructorInfo _constructor;
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

            var result = _method == null ? _constructor.Invoke(passedParms) : _method.Invoke(_obj, passedParms);
            CallAction.HandleResult(_tRet, result);
            return ExitType.Okay;
        }

        internal DotNetPrimitive(MethodInfo method, object obj = null)
        {
            _method = method;
            _tRet = _method.ReturnType;
            _tParms = _method.GetParameters().Select(pi => pi.ParameterType).ToArray();
            _obj = obj;
        }
		internal DotNetPrimitive(Delegate del) : this (del.Method) { }
    }
	#endregion
}
