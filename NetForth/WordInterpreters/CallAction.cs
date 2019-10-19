using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static NetForth.Session;

namespace NetForth.WordInterpreters
{
	static class CallAction
	{
        public static void DoCall(Tokenizer tkn, WordListBuilder wlb, bool isStatic = false)
        {
            var fname = tkn.NextToken();
            var parmsInfo = tkn.NextToken().ToLower();
            if (parmsInfo == "noprms")
            {
                parmsInfo = string.Empty;
            }
            var objIndex = Stack.Pop();
            var obj = GetObject(objIndex);
            var parms = new object[parmsInfo.Length];

            for (var iParm = parmsInfo.Length - 1; iParm >= 0; iParm--)
            {
                var val = Stack.Pop();
                switch (parmsInfo[iParm])
                {
					case 'i':
                        parms[iParm] = val;
                        break;

					case 'o':
                        parms[iParm] = GetObject(val);
                        break;

					default:
                        throw new NfException($"Invalid type in parmsInfo for call: {parmsInfo}");
                }
            }

            var parmTypes = parms.Select(o => o.GetType()).ToArray();
            MethodInfo method = null;
            if (!isStatic)
            {
                method = obj.GetType().GetMethod(fname, BindingFlags.Public | BindingFlags.Instance,
                    null, parmTypes, null);
			}
            else
            {
				// obj is our type
                method = ((Type) obj).GetMethod(fname, BindingFlags.Public | BindingFlags.Static,
                    null, parmTypes, null);
            }

			if (wlb == null)
            {
                var ret = method.Invoke(obj, parms);
                HandleResult(ret);
            }
            else
            {
                wlb.Add(new DotNetPrimitive(method));
            }
        }

        internal static void HandleResult(object result)
        {
            HandleResult(result.GetType(), result);
        }

        internal static void HandleResult(Type tRet, object result)
        {
            if (tRet == typeof(int))
            {
                Stack.Push((int)result);
            }
            else if (tRet != typeof(void))
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
