using System;
using System.Reflection;
using NetForth.Primitives;
using static NetForth.Session;

namespace NetForth.WordInterpreters
{
    static class CallAction
	{
		// ptype0 ptype1 ... cParms objType "dnName fName" => new method primitive named "fName"
		public static void CreateMethod(Tokenizer tkn)
        {
            var dnName = tkn.NextToken();
            var fName = tkn.NextToken();
            var objType = PopType($"Bad object type in definition of {fName}");
            var parmTypes = GetParmTypes($"Bad parameter type in definition of {fName}");

			var method = objType.GetMethod(dnName, BindingFlags.Public | BindingFlags.Instance,
                null, parmTypes, null);
            Vocabulary.CurrentVocabulary.AddDefinition(fName, new MethodPrimitive(method, parmTypes));
		}

		// ptype0 ptype1 ... cParms staticType "dnName fName" => new method primitive named "fName"
		public static void CreateStaticMethod(Tokenizer tkn)
        {
            var dnName = tkn.NextToken();
            var fName = tkn.NextToken();
            var staticType = PopType($"Bad static type in definition of {fName}");
            var parmTypes = GetParmTypes($"Bad parameter type in definition of {fName}");
            var method = staticType.GetMethod(dnName, BindingFlags.Public |  BindingFlags.Static,
                null, parmTypes, null);
 
           Vocabulary.CurrentVocabulary.AddDefinition(fName, new MethodPrimitive(method, parmTypes, true));
        }

        // ptype0 ptype1 ... cParms type "fName" => new method primitive named "fName"
        public static void CreateConstructor(Tokenizer tkn)
        {
            var fName = tkn.NextToken();
            var type = PopType($"Bad type in definition of constructor {fName}");
            var parmTypes = GetParmTypes($"Bad parameter type in definition of constructor {fName}");
			var ctor = type.GetConstructor(parmTypes);

			Vocabulary.CurrentVocabulary.AddDefinition(fName, new ConstructorPrimitive(ctor, parmTypes));
        }

        // ptype0 ptype1 ... cParms type "fName" => new method primitive named "fName"
		public static void CreateIndexer(Tokenizer tkn)
        {
            var fName = tkn.NextToken();
            var type = PopType($"Bad type in definition of indexer {fName}");
            var parmTypes = GetParmTypes($"Bad parameter type in definition of indexer {fName}");

            Vocabulary.CurrentVocabulary.AddDefinition(fName, new IndexerPrimitive(type, parmTypes));
        }

		internal static Type[] GetParmTypes(string errorMessage, int cParms = -1)
        {
            if (cParms < 0)
            {
                cParms = Stack.Pop();
            }

            var parmTypes = new Type[cParms];

            for (var iParm = cParms - 1; iParm >= 0; iParm--)
            {
                parmTypes[iParm] = PopType(errorMessage);
            }

            return parmTypes;
        }

        private static Type PopType(string errorMessage)
        {
            var typeIndex = Stack.Pop();
            Type retType;

            if (typeIndex == -1)
            {
                retType = null;
            }
            else
            {
                retType = GetManagedObject(typeIndex) as Type;
                if (retType == null)
                {
                    throw new NfException(errorMessage);
                }
            }

            return retType;
        }

        internal static void HandleResult(Type tRet, object result)
        {
            switch (result)
            {
				case char c:
                    Stack.Push(c);
                    return;

				case short s:
                    Stack.Push(s);
                    return;

				case int i:
                    Stack.Push(i);
                    return;
            }

            if (tRet != typeof(void))
            {
                Stack.Push(SaveManagedObject(result));
            }
            else
            {
                Stack.Push(-1);  // null
            }
        }
	}
}
