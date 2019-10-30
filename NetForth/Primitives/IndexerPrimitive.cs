using System;
using System.Linq;
using System.Reflection;
using NetForth.WordInterpreters;

namespace NetForth.Primitives
{
    internal class IndexerPrimitive : DotNetPrimitive
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
            var obj = Session.GetManagedObject(Session.Stack.Pop());
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
}